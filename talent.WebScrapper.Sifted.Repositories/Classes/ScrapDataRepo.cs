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

        public async Task<List<DetailedScrapOutputData>> FindRangeDetailedScrapDataAsync(string? sector = null)
        {
            try
            {
                if (sector == null)
                {
                    var list = await _context.DetailedScrapOutputDatas.ToListAsync();
                    return list;
                }
                else if (_context.DetailedScrapOutputDatas.Any(o => o.Sector == sector))
                {
                    var list = await _context.DetailedScrapOutputDatas.Where(x => x.Sector == sector).ToListAsync();
                    return list;
                }
                else
                {
                    return null;
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public List<string> ListOfurlsNotExistingInDb(List<string> outputDatas)
        {
            try
            {
                List<string> filteredUrls = new();
                foreach (var item in outputDatas)
                {
                    if (!_context.DetailedScrapOutputDatas.Any(o => o.articleUrl == item))
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
        public List<SectorWiseArticles> FindSectorWiseArticleUrls(string? sector)
        {
            try
            {
                if (sector == null)
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
                else
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
    }
}
