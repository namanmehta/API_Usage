using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API_Usage.Models
{
    public class Company
    {
        [Key]
        public string symbol { get; set; }
        public string name { get; set; }
        public string date { get; set; }
        public bool isEnabled { get; set; }
        public string type { get; set; }
        public string iexId { get; set; }
        public List<Equity> Equities { get; set; }
    }

    public class Equity
    {
        public int EquityId { get; set; }
        public string date { get; set; }
        public float open { get; set; }
        public float high { get; set; }
        public float low { get; set; }
        public float close { get; set; }
        public int volume { get; set; }
        public int unadjustedVolume { get; set; }
        public float change { get; set; }
        public float changePercent { get; set; }
        public float vwap { get; set; }
        public string label { get; set; }
        public float changeOverTime { get; set; }
        public string symbol { get; set; }
    }

    public class ChartRoot
    {
        public Equity[] chart { get; set; }
    }
    public class CompanyInfo
    {
        [Key]
        public string symbol { get; set; }
        public string companyName { get; set; }
        public string exchange { get; set; }
        public string industry { get; set; }
        public string website { get; set; }
        public string description { get; set; }
        public string CEO { get; set; }
        public string issueType { get; set; }
        public string sector { get; set; }
        // [NotMapped]
        public List<string> tags { get; set; }

    }
    public class Infocus
    {

        public string symbol { get; set; }
        public string companyName { get; set; }
        public string primaryExchange { get; set; }
        public string sector { get; set; }
        public string calculationPrice { get; set; }
        public string open { get; set; }
        public long openTime { get; set; }
        public float close { get; set; }
        public long closeTime { get; set; }
        public float high { get; set; }
        public double low { get; set; }
        public double latestPrice { get; set; }
        public string latestSource { get; set; }
        public string latestTime { get; set; }
        public long latestUpdate { get; set; }
        public int latestVolume { get; set; }
        public string iexRealtimePrice { get; set; }
        public string iexRealtimeSize { get; set; }
        public string iexLastUpdated { get; set; }
        public double delayedPrice { get; set; }
        public long delayedPriceTime { get; set; }
        public long extendedPrice { get; set; }
        public double extendedChange { get; set; }
        public double extendedChangePercent { get; set; }
        public long extendedPriceTime { get; set; }
        public double previousClose { get; set; }
        public double change { get; set; }
        public double changePercent { get; set; }
        public string iexMarketPercent { get; set; }
        public string iexVolume { get; set; }
        public long avgTotalVolume { get; set; }
        public string iexBidPrice { get; set; }
        public string iexBidSize { get; set; }
        public string iexAskPrice { get; set; }
        public string iexAskSize { get; set; }
        public long marketCap { get; set; }
        public string peRatio { get; set; }
        public float week52High { get; set; }
        public float week52Low { get; set; }
        public double ytdChange { get; set; }
    }
    public class CompanyData
    {
        public string date { get; set; }
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public int volume { get; set; }
        public int unadjustedVolume { get; set; }
        public double change { get; set; }
        public float changePercent { get; set; }
        public double vwap { get; set; }
        public string label { get; set; }
        public double changeOverTime { get; set; }
    }


    public class Rootobject
    {
        public News[] Property1 { get; set; }
    }

    public class News
    {
        public DateTime datetime { get; set; }
        [Key]
        public string headline { get; set; }
        public string source { get; set; }
        public string url { get; set; }
        public string summary { get; set; }
        public string related { get; set; }
        public string image { get; set; }
    }



    public class Stats
    {
        [Key]
        public string companyName { get; set; }
        public long marketcap { get; set; }
        public float beta { get; set; }
        public float week52high { get; set; }
        public int week52low { get; set; }
        public float week52change { get; set; }
        public int shortInterest { get; set; }
        public int shortDate { get; set; }
        public float dividendRate { get; set; }
        public float dividendYield { get; set; }
        public string exDividendDate { get; set; }
        public float latestEPS { get; set; }
        public string latestEPSDate { get; set; }
        public long sharesOutstanding { get; set; }
        public long _float { get; set; }
        public float returnOnEquity { get; set; }
        public float consensusEPS { get; set; }
        public int numberOfEstimates { get; set; }
        public string EPSSurpriseDollar { get; set; }
        public float EPSSurprisePercent { get; set; }
        public string symbol { get; set; }
        public int EBITDA { get; set; }
        public int revenue { get; set; }
        public int grossProfit { get; set; }
        public int cash { get; set; }
        public int debt { get; set; }
        public float ttmEPS { get; set; }
        public int revenuePerShare { get; set; }
        public int revenuePerEmployee { get; set; }
        public int peRatioHigh { get; set; }
        public float peRatioLow { get; set; }
        public float returnOnAssets { get; set; }
        public string returnOnCapital { get; set; }
        public float profitMargin { get; set; }
        public float priceToSales { get; set; }
        public float priceToBook { get; set; }
        public float day200MovingAvg { get; set; }
        public float day50MovingAvg { get; set; }
        public float institutionPercent { get; set; }
        public string insiderPercent { get; set; }
        public string shortRatio { get; set; }
        public float year5ChangePercent { get; set; }
        public float year2ChangePercent { get; set; }
        public float year1ChangePercent { get; set; }
        public float ytdChangePercent { get; set; }
        public float month6ChangePercent { get; set; }
        public float month3ChangePercent { get; set; }
        public float month1ChangePercent { get; set; }
        public float day5ChangePercent { get; set; }
        public float day30ChangePercent { get; set; }
    }




    public class Crypto
    {

        public string symbol { get; set; }
        [Key]
        public string companyName { get; set; }
        public string primaryExchange { get; set; }
        public string sector { get; set; }
        public string calculationPrice { get; set; }
        public float? open { get; set; }
        public long openTime { get; set; }
        public float? close { get; set; }
        public long closeTime { get; set; }
        public float? high { get; set; }
        public float? low { get; set; }
        public float? latestPrice { get; set; }
        public string latestSource { get; set; }
        public string latestTime { get; set; }
        public long latestUpdate { get; set; }
        public float? latestVolume { get; set; }
        public string iexRealtimePrice { get; set; }
        public string iexRealtimeSize { get; set; }
        public string iexLastUpdated { get; set; }
        public string delayedPrice { get; set; }
        public string delayedPriceTime { get; set; }
        public string extendedPrice { get; set; }
        public string extendedChange { get; set; }
        public string extendedChangePercent { get; set; }
        public string extendedPriceTime { get; set; }
        public string previousClose { get; set; }
        public string change { get; set; }
        public string changePercent { get; set; }
        public string iexMarketPercent { get; set; }
        public string iexVolume { get; set; }
        public long? avgTotalVolume { get; set; }
        public string iexBidPrice { get; set; }
        public string iexBidSize { get; set; }
        public string iexAskPrice { get; set; }
        public string iexAskSize { get; set; }
        public string marketCap { get; set; }
        public string peRatio { get; set; }
        public string week52High { get; set; }
        public string week52Low { get; set; }
        public string ytdChange { get; set; }
        public string bidPrice { get; set; }
        public string bidSize { get; set; }
        public string askPrice { get; set; }
        public string askSize { get; set; }
    }


    

    public class Sector
    {
        public string type { get; set; }
        [Key]
        public string name { get; set; }
        public float performance { get; set; }
        public long lastUpdated { get; set; }
    }


    

    public class GainersList
    {
        public string symbol { get; set; }
        [Key]
        public string companyName { get; set; }
        public string primaryExchange { get; set; }
        public string sector { get; set; }
        public string calculationPrice { get; set; }
        public float? open { get; set; }
        public long? openTime { get; set; }
        public float? close { get; set; }
        public long? closeTime { get; set; }
        public float? high { get; set; }
        public float? low { get; set; }
        public float? latestPrice { get; set; }
        public string latestSource { get; set; }
        public string latestTime { get; set; }
        public long? latestUpdate { get; set; }
        public int latestVolume { get; set; }
        public float? iexRealtimePrice { get; set; }
        public int? iexRealtimeSize { get; set; }
        public long? iexLastUpdated { get; set; }
        public float? delayedPrice { get; set; }
        public long? delayedPriceTime { get; set; }
        public float? extendedPrice { get; set; }
        public float? extendedChange { get; set; }
        public float? extendedChangePercent { get; set; }
        public long? extendedPriceTime { get; set; }
        public float? previousClose { get; set; }
        public float? change { get; set; }
        public float? changePercent { get; set; }
        public float? iexMarketPercent { get; set; }
        public int? iexVolume { get; set; }
        public int avgTotalVolume { get; set; }
        public int? iexBidPrice { get; set; }
        public int? iexBidSize { get; set; }
        public int? iexAskPrice { get; set; }
        public int? iexAskSize { get; set; }
        public long? marketCap { get; set; }
        public float? peRatio { get; set; }
        public float week52High { get; set; }
        public string week52Low { get; set; }
        public float ytdChange { get; set; }
    }


}