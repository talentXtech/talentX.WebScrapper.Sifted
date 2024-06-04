using Microsoft.EntityFrameworkCore;
using talentX.WebScrapper.Sifted.Entities;

namespace talentX.WebScrapper.Sifted.Repositories.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<DetailedScrapOutputData> DetailedScrapOutputDatas { get; set; }
        public DbSet<InitialScrapOutputData> InitialScrapOutputDatas { get; set; }

        public DbSet<SectorWiseArticles> SectorWiseArticles { get; set; }
    }
}
