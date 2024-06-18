using Microsoft.EntityFrameworkCore;
using talentX.WebScrapper.Sifted.Entities;
using talentX.WebScrapper.Sifted.Repositories.Contracts;
using talentX.WebScrapper.Sifted.Repositories.Data;

namespace talentX.WebScrapper.Sifted.Repositories.Classes
{
    public class ScrapDataRepo : IScrapDataRepo
    {
        private readonly DataContext _context;

        public ScrapDataRepo(DataContext context)
        {
            _context = context;

        }
        public async Task AddRangeInitialScrapDataAsync(List<InitialScrapOutputData> outputDatas)
        {
            try
            {
                List<InitialScrapOutputData> filteredDatas = new();
                foreach (var item in outputDatas)
                {

                    if (!_context.InitialScrapOutputDatas.Any(o => o.SectorUrl == item.SectorUrl))
                    {
                        filteredDatas.Add(item);
                    }
                }
                await _context.InitialScrapOutputDatas.AddRangeAsync(filteredDatas);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task DeleteInitialScrapDataAsync()
        {
            try
            {
                _context.Database.ExecuteSqlRaw("TRUNCATE TABLE InitialScrapOutputDatas");
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task AddSectorWiseArticle(SectorWiseArticles articles)
        {
            try
            {
                await _context.SectorWiseArticles.AddRangeAsync(articles);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task AddDetailedScrapDataAsync(DetailedScrapOutputData outputData)
        {
            try
            {

                {
                    await _context.DetailedScrapOutputDatas.AddAsync(outputData);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task AddRangeDetailedScrapDataAsync(List<DetailedScrapOutputData> outputDatas)
        {
            try
            {
                await _context.DetailedScrapOutputDatas.AddRangeAsync(outputDatas);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task DeleteDetailedScrapDataAsync()
        {
            try
            {
                _context.Database.ExecuteSqlRaw("TRUNCATE TABLE DetailedScrapOutputDatas");
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task DeleteSectorTableScrapDataAsync()
        {
            try
            {
                _context.Database.ExecuteSqlRaw("TRUNCATE TABLE SectorWiseArticles");
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task DeleteInitialScrapDataBySectorAsync(string sector)
        {
            try
            {
                var listOfDataToDelete = await _context.InitialScrapOutputDatas.Where(x => x.Sectors == sector).ToListAsync();
                _context.InitialScrapOutputDatas.RemoveRange(listOfDataToDelete);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
        public async Task DeleteDetailedScrapDataBySectorAsync(string sector)
        {
            try
            {
                var listOfDataToDelete = await _context.DetailedScrapOutputDatas.Where(x => x.Sector == sector).ToListAsync();
                _context.DetailedScrapOutputDatas.RemoveRange(listOfDataToDelete);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
        public async Task DeleteSectorTableScrapDataBySectorAsync(string sector)
        {
            try
            {
                var listOfDataToDelete = await _context.SectorWiseArticles.Where(x => x.Sectors == sector).ToListAsync();
                _context.SectorWiseArticles.RemoveRange(listOfDataToDelete);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<DetailedScrapOutputData>> FindRangeDetailedScrapDataAsync()
        {
            try
            {
                    var list = await _context.DetailedScrapOutputDatas.ToListAsync();
                    return list;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task<List<DetailedScrapOutputData>> FindRangeDetailedScrapDataBySectorAsync(string sector)
        {
                var list = await _context.DetailedScrapOutputDatas.Where(x => x.Sector == sector).ToListAsync();
                return list;

        }

        public List<SectorWiseArticles> ListOfurlsNotExistingInDb(List<SectorWiseArticles> outputDatas)
        {
            try
            {
                List<SectorWiseArticles> filteredUrls = new();
                foreach (var item in outputDatas)
                {
                    if (!_context.DetailedScrapOutputDatas.Any(o => o.articleUrl == item.ArticleUrl))
                    {
                        filteredUrls.Add(item);
                    }

                }
                return filteredUrls;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public List<string> ListOfurlsNotExistingInSectorWiseArticleList(List<string> outputDatas)
        {
            try
            {
                List<string> filteredUrls = new();
                foreach (var item in outputDatas)
                {
                    if (!_context.SectorWiseArticles.Any(o => o.ArticleUrl == item))
                    {
                        filteredUrls.Add(item);
                    }

                }
                return filteredUrls;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
        public List<SectorWiseArticles> FindSectorWiseArticleUrls()
        {
            try
            {
                    var list = _context.SectorWiseArticles.ToList();
                    List<SectorWiseArticles> filteredList = new();
                    foreach (var item in list)
                    {
                        if (!_context.DetailedScrapOutputDatas.Any(o => o.articleUrl == item.ArticleUrl))
                        {
                            filteredList.Add(item);
                        }
                    }
                    return filteredList;
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public List<SectorWiseArticles> FindSectorWiseArticleUrlsBasedOnSector(string sector)
        {
            try
            {
                var list = _context.SectorWiseArticles.Where(o => o.Sectors == sector).ToList();
                List<SectorWiseArticles> filteredList = new();
                foreach (var item in list)
                {
                    if (!_context.DetailedScrapOutputDatas.Any(o => o.articleUrl == item.ArticleUrl))
                    {
                        filteredList.Add(item);
                    }
                }
                return filteredList;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public List<InitialScrapOutputData> FindInitialScrapOutputData()
        {
            try
            {
                var list = _context.InitialScrapOutputDatas.ToList();
                return list;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<string>> GetFiltersByCategory()
        {
            var categories = await _context.SectorWiseArticles.Select(p => p.Sectors).Distinct().ToListAsync();
            return categories;
        }

        public async Task<List<string>> GetListOfSectorsToDownloadDataByCategory()
        {
            var categories = await _context.DetailedScrapOutputDatas.Select(p => p.Sector).Distinct().ToListAsync();
            return categories;
        }
    }
}
