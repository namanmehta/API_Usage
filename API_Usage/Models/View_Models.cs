using System.Collections.Generic;
//using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API_Usage.Models
{
  public class CompaniesEquities
  {
    public List<Company> Companies { get; set; }
    public Equity Current { get; set; }
    public string Dates { get; set; }
    public string Prices { get; set; }
    public string Volumes { get; set; }
    public float AvgPrice { get; set; }
    public double AvgVolume { get; set; }

    public CompaniesEquities(List<Company> companies, Equity current,
                                      string dates, string prices, string volumes,
                                      float avgprice, double avgvolume)
    {
      Companies = companies;
      Current = current;
      Dates = dates;
      Prices = prices;
      Volumes = volumes;
      AvgPrice = avgprice;
      AvgVolume = avgvolume;
    }
  }

  public class AzureMLModel
  {
    public string Message { get; set; }
    public string JsonObject { get; set; }
  }

    public class sectorData
    {
        [Key]
        public int id { get; set; }
        public List<GainersList> Gain { get; set; }
        public List<Sector> SectorL { get; set; }
    }

    public class financialData
    {
        public FinancialList finance { get; set; }
        public List<Company> company { get; set; }
    }
}