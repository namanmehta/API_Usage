using Microsoft.AspNetCore.Mvc;
using API_Usage.DataAccess;
using API_Usage.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;

/*
 * Acknowledgments
 *  v1 of the project was created for the Fall 2018 class by Dhruv Dhiman, MS BAIS '18
 *    This example showed how to use v1 of the IEXTrading API
 *    
 *  Kartikay Bali (MS BAIS '19) extended the project for Spring 2019 by demonstrating 
 *    how to use similar methods to access Azure ML models
*/

namespace API_Usage.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext dbContext;

        //Base URL for the IEXTrading API. Method specific URLs are appended to this base URL.
        string BASE_URL = "https://api.iextrading.com/1.0/";
        HttpClient httpClient;

        /// <summary>
        /// Initialize the database connection and HttpClient object
        /// </summary>
        /// <param name="context"></param>
        public HomeController(ApplicationDbContext context)
        {
            dbContext = context;

            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new
                System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public IActionResult Index()
        {
            return View();
        }

        /****
         * The Symbols action calls the GetSymbols method that returns a list of Companies.
         * This list of Companies is passed to the Symbols View.
        ****/
        public IActionResult Symbols()
        {
            //Set ViewBag variable first
            ViewBag.dbSucessComp = 0;
            List<Company> companies1 = GetSymbols1();

            //Save companies in TempData, so they do not have to be retrieved again
            TempData["Companies"] = JsonConvert.SerializeObject(companies1);
            //TempData["Companies"] = companies;

            return View(companies1);
        }


        
        public IActionResult Chart(string symbol)
        {
            //Set ViewBag variable first
            ViewBag.dbSuccessChart = 0;
            List<Equity> equities = new List<Equity>();

            if (symbol != null)
            {
                equities = GetChart(symbol);
                equities = equities.OrderBy(c => c.date).ToList(); //Make sure the data is in ascending order of date.
            }

            CompaniesEquities companiesEquities = getCompaniesEquitiesModel(equities);

            return View(companiesEquities);
        }

        /// <summary>
        /// Calls the IEX reference API to get the list of symbols
        /// </summary>
        /// <returns>A list of the companies whose information is available</returns>
        public List<Company> GetSymbols()
        {
            string IEXTrading_API_PATH = BASE_URL + "ref-data/symbols";
            string companyList = "";
            List<Company> companies = null;

            // connect to the IEXTrading API and retrieve information
            httpClient.BaseAddress = new Uri(IEXTrading_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_PATH).GetAwaiter().GetResult();

            // read the Json objects in the API response
            if (response.IsSuccessStatusCode)
            {
                companyList = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            // now, parse the Json strings as C# objects
            if (!companyList.Equals(""))
            {
                // https://stackoverflow.com/a/46280739
                //JObject result = JsonConvert.DeserializeObject<JObject>(companyList);
                companies = JsonConvert.DeserializeObject<List<Company>>(companyList);
                companies = companies.GetRange(0, 50);
            }

            return companies;
        }


        public List<Company> GetSymbols1()
        {
            string IEXTrading_API_PATH = BASE_URL + "ref-data/symbols";
            string companyList = "";

            List<Company> companies = null;

            httpClient.BaseAddress = new Uri(IEXTrading_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_PATH).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                companyList = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            if (!companyList.Equals(""))
            {
                companies = JsonConvert.DeserializeObject<List<Company>>(companyList);
                companies = companies.GetRange(0, 9);
            }

            return companies;
        }


        /// <summary>
        /// Calls the IEX stock API to get 1 year's chart for the supplied symbol
        /// </summary>
        /// <param name="symbol">Stock symbol of the company whose quotes are to be retrieved</param>
        /// <returns></returns>
        public List<Equity> GetChart(string symbol)
        {
            // string to specify information to be retrieved from the API
            string IEXTrading_API_PATH = BASE_URL + "stock/" + symbol + "/batch?types=chart&range=1y";

            // initialize objects needed to gather data
            string charts = "";
            List<Equity> Equities = new List<Equity>();
            httpClient.BaseAddress = new Uri(IEXTrading_API_PATH);

            // connect to the API and obtain the response
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_PATH).GetAwaiter().GetResult();

            // now, obtain the Json objects in the response as a string
            if (response.IsSuccessStatusCode)
            {
                charts = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            // parse the string into appropriate objects
            if (!charts.Equals(""))
            {
                ChartRoot root = JsonConvert.DeserializeObject<ChartRoot>(charts,
                  new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Equities = root.chart.ToList();
            }

            // fix the relations. By default the quotes do not have the company symbol
            //  this symbol serves as the foreign key in the database and connects the quote to the company
            foreach (Equity Equity in Equities)
            {
                Equity.symbol = symbol;
            }

            return Equities;
        }

        /// <summary>
        /// Call the ClearTables method to delete records from a table or all tables.
        ///  Count of current records for each table is passed to the Refresh View
        /// </summary>
        /// <param name="tableToDel">Table to clear</param>
        /// <returns>Refresh view</returns>
        public IActionResult Refresh(string tableToDel)
        {
            ClearTables(tableToDel);
            Dictionary<string, int> tableCount = new Dictionary<string, int>();
            tableCount.Add("Companies", dbContext.Companies.Count());
            tableCount.Add("Charts", dbContext.Equities.Count());
            tableCount.Add("News", dbContext.News.Count());
            tableCount.Add("Cryto", dbContext.Crypto.Count());
            tableCount.Add("Sector", dbContext.Sector.Count());
            tableCount.Add("Statistics", dbContext.Stats.Count());
            


            return View(tableCount);
        }

        /// <summary>
        /// save the quotes (equities) in the database
        /// </summary>
        /// <param name="symbol">Company whose quotes are to be saved</param>
        /// <returns>Chart view for the company</returns>
        public IActionResult SaveCharts(string symbol)
        {
            List<Equity> equities = GetChart(symbol);

            // save the quote if the quote has not already been saved in the database
            foreach (Equity equity in equities)
            {
                if (dbContext.Equities.Where(c => c.date.Equals(equity.date)).Count() == 0)
                {
                    dbContext.Equities.Add(equity);
                }
            }

            // persist the data
            dbContext.SaveChanges();

            // populate the models to render in the view
            ViewBag.dbSuccessChart = 1;
            CompaniesEquities companiesEquities = getCompaniesEquitiesModel(equities);
            return View("Chart", companiesEquities);
        }
        public IActionResult News()
        {
            ViewBag.dbSucessComp = 0;

            string IEXTrading_API_PATH = BASE_URL + "stock/market/news/last/5";
            string newsL = "";
            List<News> newsList = null;

            // connect to the IEXTrading API and retrieve information
            //httpClient.BaseAddress = new Uri(IEXTrading_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_PATH).GetAwaiter().GetResult();

            // read the Json objects in the API response
            if (response.IsSuccessStatusCode)
            {
                newsL = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            // now, parse the Json strings as C# objects
            if (!newsL.Equals(""))
            {
                // https://stackoverflow.com/a/46280739
                //JObject result = JsonConvert.DeserializeObject<JObject>(companyList);
                newsList = JsonConvert.DeserializeObject<List<News>>(newsL);
                // companies = companies.GetRange(0, 50);
            }

            TempData["News"] = JsonConvert.SerializeObject(newsList);


            return View(newsList);
        }

        public IActionResult Stats()
        {
            ViewBag.dbSucessComp = 0;
            //string a = "aapl";
            string IEXTrading_API_PATH = BASE_URL + "stock/aapl/stats";
            string statsL = "";
            Stats statslist = null;


            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_PATH).GetAwaiter().GetResult();

            // read the Json objects in the API response
            if (response.IsSuccessStatusCode)
            {
                statsL = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            // now, parse the Json strings as C# objects
            if (!statsL.Equals(""))
            {
                // https://stackoverflow.com/a/46280739
                //JObject result = JsonConvert.DeserializeObject<JObject>(companyList);
                statslist = JsonConvert.DeserializeObject<Stats>(statsL);
                // companies = companies.GetRange(0, 50);
            }

            TempData["Stats"] = JsonConvert.SerializeObject(statslist);


            return View(statslist);
        }

        public IActionResult PopulateStats()
        {

            Stats stats1 = JsonConvert.DeserializeObject<Stats>(TempData["Stats"].ToString());

            //foreach (Stats new1 in stats1)
            //{

            if (dbContext.Companies.Where(c => c.symbol.Equals(stats1.companyName)).Count() == 0)
            {
                dbContext.Stats.Add(stats1);
            }
            //}

            dbContext.SaveChanges();
            ViewBag.dbSuccessComp = 1;
            return View("Stats", stats1);
        }

        public IActionResult PopulateNews()
        {

            List<News> news1 = JsonConvert.DeserializeObject<List<News>>(TempData["News"].ToString());

            foreach (News new1 in news1)
            {

                if (dbContext.Companies.Where(c => c.symbol.Equals(new1.headline)).Count() == 0)
                {
                    dbContext.News.Add(new1);
                }
            }

            dbContext.SaveChanges();
            ViewBag.dbSuccessComp = 1;
            return View("News", news1);
        }



        public IActionResult Crypto()
        {
            ViewBag.dbSucessComp = 0;

            string IEXTrading_API_PATH = BASE_URL + "stock/market/crypto";
            string cryptoL = "";
            List<Crypto> cryptoList = null;

            // connect to the IEXTrading API and retrieve information
            //httpClient.BaseAddress = new Uri(IEXTrading_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_PATH).GetAwaiter().GetResult();

            // read the Json objects in the API response
            if (response.IsSuccessStatusCode)
            {
                cryptoL = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            // now, parse the Json strings as C# objects
            if (!cryptoL.Equals(""))
            {
                // https://stackoverflow.com/a/46280739
                //JObject result = JsonConvert.DeserializeObject<JObject>(companyList);
                cryptoList = JsonConvert.DeserializeObject<List<Crypto>>(cryptoL);
                // companies = companies.GetRange(0, 50);
            }
            TempData["Crypto"] = JsonConvert.SerializeObject(cryptoList.Take(10));


            return View(cryptoList);
        }

        public IActionResult PopulateCrypt()
        {

            List<Crypto> crypto1 = JsonConvert.DeserializeObject<List<Crypto>>(TempData["Crypto"].ToString());

            foreach (Crypto new1 in crypto1)
            {

                if (dbContext.Companies.Where(c => c.symbol.Equals(new1.companyName)).Count() == 0)
                {
                    dbContext.Crypto.Add(new1);
                }
            }

            dbContext.SaveChanges();
            ViewBag.dbSuccessComp = 1;
            return View("Crypto", crypto1);
        }

        public IActionResult Sector()
        {
            ViewBag.dbSucessComp = 0;

            string IEXTrading_API_PATH = BASE_URL + "stock/market/sector-performance";
            string sectorL = "";
            List<Sector> sectorList = null;

            // connect to the IEXTrading API and retrieve information
            //httpClient.BaseAddress = new Uri(IEXTrading_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_PATH).GetAwaiter().GetResult();

            // read the Json objects in the API response
            if (response.IsSuccessStatusCode)
            {
                sectorL = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            // now, parse the Json strings as C# objects
            if (!sectorL.Equals(""))
            {
                // https://stackoverflow.com/a/46280739
                //JObject result = JsonConvert.DeserializeObject<JObject>(companyList);
                sectorList = JsonConvert.DeserializeObject<List<Sector>>(sectorL);
                // companies = companies.GetRange(0, 50);
            }

            TempData["Sector"] = JsonConvert.SerializeObject(sectorList.Take(10));


            return View(sectorList);
        }

        public IActionResult PopulateSector()
        {

            List<Sector> s1 = JsonConvert.DeserializeObject<List<Sector>>(TempData["Sector"].ToString());

            foreach (Sector new1 in s1)
            {

                if (dbContext.Companies.Where(c => c.symbol.Equals(new1.name)).Count() == 0)
                {
                    dbContext.Sector.Add(new1);
                }
            }

            dbContext.SaveChanges();
            ViewBag.dbSuccessComp = 1;
            return View("Sector", s1);
        }


        public IActionResult Top5()
        {
            //Set ViewBag variable first
            ViewBag.dbSucessComp = 0;

            List<CompanyInfo> companies = GetTopFiveSymbols();

            //Save comapnies in TempData
            TempData["topCompanies"] = JsonConvert.SerializeObject(companies);

            return View(companies);
        }

        public List<CompanyInfo> GetTopFiveSymbols()
        {
            ///
            //HttpClient httpClient;
            //httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Accept.Clear();
            //httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            ////
            ///
            string IEXTrading_STOCK_DATA_API_PATH = BASE_URL + "stock/{0}/chart/1m";


            List<String> companiesSymbolList = GetInfocusSymbols();

            //Dictionary<string, List<CompanyData>> companyDict = new Dictionary<string, List<CompanyData>>();
            Dictionary<string, float> changePercentDict = new Dictionary<string, float>();

            foreach (var companySymbol in companiesSymbolList)
            {
                IEXTrading_STOCK_DATA_API_PATH = BASE_URL + "stock/{0}/chart/1m";
                float changePercent = 0;
                HttpClient httpClient;
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string companyDataString = "";
                IEXTrading_STOCK_DATA_API_PATH = string.Format(IEXTrading_STOCK_DATA_API_PATH, companySymbol);
                httpClient.BaseAddress = new Uri(IEXTrading_STOCK_DATA_API_PATH);
                HttpResponseMessage response = httpClient.GetAsync(IEXTrading_STOCK_DATA_API_PATH).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    companyDataString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                if (!companyDataString.Equals(""))
                {

                    List<CompanyData> companyOneMonthData = new List<CompanyData>();
                    companyOneMonthData = JsonConvert.DeserializeObject<List<CompanyData>>(companyDataString);
                    foreach (var companyOneDay in companyOneMonthData)
                    {
                        changePercent += companyOneDay.changePercent;
                    }
                    changePercent = changePercent / companyOneMonthData.Count;
                }
                changePercentDict.Add(companySymbol, changePercent);

            }

            var MaxFive = from entry in changePercentDict orderby entry.Value descending select entry;
            List<CompanyInfo> companiesList = new List<CompanyInfo>();

            foreach (var companyData in MaxFive.Take(5))
            {
                string IEXTrading_COMPANY_PROFILE_API_PATH = BASE_URL + "stock/{0}/company";
                string companySymbol = companyData.Key;
                HttpClient httpClient;
                CompanyInfo companyInfo = new CompanyInfo();
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                string companyDataString = "";
                IEXTrading_COMPANY_PROFILE_API_PATH = string.Format(IEXTrading_COMPANY_PROFILE_API_PATH, companySymbol);
                httpClient.BaseAddress = new Uri(IEXTrading_COMPANY_PROFILE_API_PATH);
                HttpResponseMessage response = httpClient.GetAsync(IEXTrading_COMPANY_PROFILE_API_PATH).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    companyDataString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                if (!companyDataString.Equals(""))
                {

                    companyInfo = JsonConvert.DeserializeObject<CompanyInfo>(companyDataString);

                }
                companiesList.Add(companyInfo);
            }


            return companiesList;
        }
        public List<String> GetInfocusSymbols()
        {
            List<String> popularSymbols = new List<String>();
            ////
            HttpClient httpClient;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            ///
            List<Infocus> InfocusList = new List<Infocus>();
            string IEXTrading_GAINERS_API_PATH = BASE_URL + "stock/market/list/infocus";
            httpClient.BaseAddress = new Uri(IEXTrading_GAINERS_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_GAINERS_API_PATH).GetAwaiter().GetResult();
            string companies = "";
            if (response.IsSuccessStatusCode)
            {
                companies = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            if (companies != null || !companies.Equals(""))
            {
                InfocusList = JsonConvert.DeserializeObject<List<Infocus>>(companies);
            }


            foreach (var company in InfocusList)
            {
                popularSymbols.Add(company.symbol);
            }

            return popularSymbols;
        }

        public CompaniesEquities getCompaniesEquitiesModel(List<Equity> equities)
        {
            List<Company> companies = dbContext.Companies.ToList();

            if (equities.Count == 0)
            {
                return new CompaniesEquities(companies, null, "", "", "", 0, 0);
            }

            Equity current = equities.Last();

            // create appropriately formatted strings for use by chart.js
            string dates = string.Join(",", equities.Select(e => e.date));
            string prices = string.Join(",", equities.Select(e => e.high));
            float avgprice = equities.Average(e => e.high);

            //Divide volumes by million to scale appropriately
            string volumes = string.Join(",", equities.Select(e => e.volume / 1000000));
            double avgvol = equities.Average(e => e.volume) / 1000000;

            return new CompaniesEquities(companies, equities.Last(), dates, prices, volumes, avgprice, avgvol);
        }

        /// <summary>
        /// Save the available symbols in the database
        /// </summary>
        /// <returns></returns>
        public IActionResult PopulateSymbols()
        {
            // retrieve the companies that were saved in the symbols method
            // saving in TempData is extremely inefficient - the data circles back from the browser
            // better methods would be to serialize to the hard disk, or save directly into the database
            //  in the symbols method. This example has been structured to demonstrate one way to save object data
            //  and retrieve it later
            List<Company> companies = JsonConvert.DeserializeObject<List<Company>>(TempData["Companies"].ToString());

            foreach (Company company in companies)
            {
                //Database will give PK constraint violation error when trying to insert record with existing PK.
                //So add company only if it doesnt exist, check existence using symbol (PK)
                if (dbContext.Companies.Where(c => c.symbol.Equals(company.symbol)).Count() == 0)
                {
                    dbContext.Companies.Add(company);
                }
            }

            dbContext.SaveChanges();
            ViewBag.dbSuccessComp = 1;
            return View("Symbols", companies);
        }


        /// <summary>
        /// Delete all records from tables
        /// </summary>
        /// <param name="tableToDel">Table to clear</param>
        public void ClearTables(string tableToDel)
        {
            if ("all".Equals(tableToDel))
            {
                //First remove equities and then the companies
                dbContext.Equities.RemoveRange(dbContext.Equities);
                dbContext.Companies.RemoveRange(dbContext.Companies);
                dbContext.Stats.RemoveRange(dbContext.Stats);
                dbContext.News.RemoveRange(dbContext.News);
                dbContext.Crypto.RemoveRange(dbContext.Crypto);
                dbContext.Sector.RemoveRange(dbContext.Sector);


            }
            else if ("Companies".Equals(tableToDel))
            {
                //Remove only those companies that don't have related quotes stored in the Equities table
                dbContext.Companies.RemoveRange(dbContext.Companies
                                                         .Where(c => c.Equities.Count == 0)
                                                                      );
            }
            else if ("Charts".Equals(tableToDel))
            {
                dbContext.Equities.RemoveRange(dbContext.Equities);
            }
            else if("News".Equals(tableToDel))
            {
                dbContext.News.RemoveRange(dbContext.News);

            }
            else if ("Crypto".Equals(tableToDel))
            {
                dbContext.Crypto.RemoveRange(dbContext.Crypto);

            }
            else if ("Sector".Equals(tableToDel))
            {
                dbContext.Sector.RemoveRange(dbContext.Sector);

            }
            else if ("Statistics".Equals(tableToDel))
            {
                dbContext.Stats.RemoveRange(dbContext.Stats);

            }

            dbContext.SaveChanges();
        }

        public IActionResult About()
        {
            return View("about");
        }

        public IActionResult Sector1(string sector)
        {
            ViewBag.dbSucessComp = 0;

            sectorData sectorList = new sectorData();

            sectorList.SectorL = getSector();
            if (sector != null)
            {
                sectorList.Gain = getSectorStock(sector);
            }
            else
            {
                sectorList.Gain = getSectorStock("Health%20Care");
            }
            TempData["SectorA"] = JsonConvert.SerializeObject(sectorList.Gain.Take(10));

            //TempData["Sector"] = JsonConvert.SerializeObject(sectorList.Take(10));

            return View("Sector_Stock", sectorList);
        }

        public IActionResult PopulateSector1()
        {

            List<sectorData> s1 = JsonConvert.DeserializeObject<List<sectorData>>(TempData["SectorA"].ToString());

            foreach (sectorData new1 in s1)
            {

                if (dbContext.Companies.Where(c => c.symbol.Equals(new1.SectorL)).Count() == 0)
                {
                    dbContext.sectorData.Add(new1);
                }
            }

            dbContext.SaveChanges();
            ViewBag.dbSuccessComp = 1;
            return View("Sector_Stock", s1);
        }

        public List<Sector> getSector()
        {
            string IEXTrading_API_PATH = BASE_URL + "stock/market/sector-performance";
            string sList = "";
            List<Sector> sectorList = null;

            // connect to the IEXTrading API and retrieve information
            //httpClient.BaseAddress = new Uri(IEXTrading_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_PATH).GetAwaiter().GetResult();

            // read the Json objects in the API response
            if (response.IsSuccessStatusCode)
            {
                sList = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            if (!sList.Equals(""))
            {
                sectorList = JsonConvert.DeserializeObject<List<Sector>>(sList, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            }
            return sectorList;
        }

        public List<GainersList> getSectorStock(string sector)
        {
            string IEXTrading_API_PATH = BASE_URL + "stock/market/collection/sector?collectionName=" + sector;
            string gList = "";
            List<GainersList> sectorStock = null;

            // connect to the IEXTrading API and retrieve information
            //httpClient.BaseAddress = new Uri(IEXTrading_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(IEXTrading_API_PATH).GetAwaiter().GetResult();

            // read the Json objects in the API response
            if (response.IsSuccessStatusCode)
            {
                gList = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            if (!gList.Equals(""))
            {
                sectorStock = JsonConvert.DeserializeObject<List<GainersList>>(gList, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            }
            return sectorStock;
        }
    }
}