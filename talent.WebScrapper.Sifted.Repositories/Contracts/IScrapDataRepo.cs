using talentX.WebScrapper.Sifted.Entities;

namespace talentX.WebScrapper.Sifted.Repositories.Contracts
{
    public interface IScrapDataRepo
    {
        Task AddRangeInitialScrapDataAsync(List<InitialScrapOutputData> outputDatas);
        Task DeleteInitialScrapDataAsync();

        Task AddSectorWiseArticle(SectorWiseArticles articles);
        Task AddDetailedScrapDataAsync(DetailedScrapOutputData outputData);
        Task AddRangeDetailedScrapDataAsync(List<DetailedScrapOutputData> outputDatas);
        Task DeleteDetailedScrapDataAsync();
        Task<List<DetailedScrapOutputData>> FindRangeDetailedScrapDataAsync(string? sector);
        List<string> ListOfurlsNotExistingInDb(List<string> outputDatas);

        List<SectorWiseArticles> FindSectorWiseArticleUrls(string? sector);

        List<InitialScrapOutputData> FindInitialScrapOutputData();
    }
}
