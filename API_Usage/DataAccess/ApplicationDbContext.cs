using Microsoft.EntityFrameworkCore;
using API_Usage.Models;

namespace API_Usage.DataAccess
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Equity> Equities { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Crypto> Crypto { get; set; }
    public DbSet<Sector> Sector { get; set; }
    public DbSet<Stats> Stats { get; set; }
    public DbSet<sectorData> sectorData { get; set; }
    public DbSet<FinancialList> FinancialList { get; set; }
    public DbSet<Financial> Financial { get; set; }
    }
}