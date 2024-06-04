using talentX.WebScrapper.Sifted.Entities;

namespace talentX.WebScrapper.Sifted.Repositories.Data
{
    public class DbInitializer
    {
        public static void Initialize(DataContext context)
        {
            context.SaveChanges();
        }
    }
}
