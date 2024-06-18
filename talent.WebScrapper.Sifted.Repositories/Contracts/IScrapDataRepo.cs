using talentX.WebScrapper.Sifted.Entities;

namespace talentX.WebScrapper.Sifted.Repositories.Contracts
{
    public interface IScrapDataRepo
    {
        Task AddRangeInitialScrapDataAsync(List<InitialScrapOutputData> outputDatas);
        
        Task AddSectorWiseArticle(SectorWiseArticles articles);
        Task AddDetailedScrapDataAsync(DetailedScrapOutputData outputData);
        Task AddRangeDetailedScrapDataAsync(List<DetailedScrapOutputData> outputDatas);

        Task DeleteInitialScrapDataAsync();
        Task DeleteDetailedScrapDataAsync();

        Task DeleteSectorTableScrapDataAsync();

        Task DeleteInitialScrapDataBySectorAsync(string sector);
        Task DeleteDetailedScrapDataBySectorAsync(string sector);
        Task DeleteSectorTableScrapDataBySectorAsync(string sector);
        Task<List<DetailedScrapOutputData>> FindRangeDetailedScrapDataAsync();

        Task<List<DetailedScrapOutputData>> FindRangeDetailedScrapDataBySectorAsync(string? sector);
        List<SectorWiseArticles> ListOfurlsNotExistingInDb(List<SectorWiseArticles> outputDatas);

        List<string> ListOfurlsNotExistingInSectorWiseArticleList(List<string> outputDatas);

        List<SectorWiseArticles> FindSectorWiseArticleUrlsBasedOnSector(string sector);
        List<SectorWiseArticles> FindSectorWiseArticleUrls();

        List<InitialScrapOutputData> FindInitialScrapOutputData();

        Task<List<string>> GetFiltersByCategory();

        Task<List<string>> GetListOfSectorsToDownloadDataByCategory();

    }
}
