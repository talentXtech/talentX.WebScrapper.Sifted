using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;
using System.Formats.Asn1;
using System.Globalization;
using talentX.WebScrapper.Sifted.Entities;
using talentX.WebScrapper.Sifted.Extensions;
using talentX.WebScrapper.Sifted.Repositories.Contracts;
using talentX.WebScrapper.Sifted.Utils;

namespace talentX.WebScrapper.Sifted.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebScrapController : ControllerBase
    {
        private readonly IScrapDataRepo _scrapDataRepo;
        private readonly IConfiguration _config;
        private readonly string _email;
        private readonly string _password;
        private readonly ILogger<WebScrapController> _logger;

        public WebScrapController(IScrapDataRepo scrapDataRepo, IConfiguration config, ILogger<WebScrapController> logger)
        {
            _scrapDataRepo = scrapDataRepo;
            _config = config;
            _email = _config.GetValue<string>("email");
            _password = _config.GetValue<string>("password");
            _logger = logger;
        }

        [HttpPost("ScrapInitialInfo")]
        public async Task<ActionResult> SiftedScrapInfo()
        {
            var ListOfInitialScrapDataFromSifted = new List<InitialScrapOutputData>();
            var driver = ChromeDriverUtils.CreateChromeDriver("https://sifted.eu/sectors");

            try
            {
                Thread.Sleep(2000);
                ChromeDriverUtils.CloseComplainaceOverlay(driver);

                ChromeDriverUtils.UserLogin(driver, _email, _password);

                Thread.Sleep(5000);
                ChromeDriverUtils.CloseComplainaceOverlay(driver);

                ChromeDriverUtils.ScrollToBottmOfPage(driver);
                Thread.Sleep(2000);

                await ScrapSectorsAndSectorUrls(ListOfInitialScrapDataFromSifted, driver);

                await ScrapSectorWiseArticleUrls(driver);

                var apiResponse = ResponseUtils.GetSuccesfulResponse("Data Scrapped scuccesfully and is ready to proceed further!");
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
            finally
            {
                driver.Quit();
            }
        }

        [HttpPost("ScrapDetailedScrapInfoBasedOnSector")]
        public async Task<IActionResult> ScrapInfoFromEachArticleUrlBasedOnSector(string? sector = null)
        {

            var driver = ChromeDriverUtils.CreateChromeDriver("https://sifted.eu/sectors");
            int noOfDataToBeScrapped = 0;
            int dataScrapped = 0;
     
            try
            {
                Thread.Sleep(2000);
                ChromeDriverUtils.CloseComplainaceOverlay(driver);

                ChromeDriverUtils.UserLogin(driver,  _email, _password);

                Thread.Sleep(5000);
                ChromeDriverUtils.CloseComplainaceOverlay(driver);

                var articleData = _scrapDataRepo.FindSectorWiseArticleUrlsBasedOnSector(sector);
                var filteredArticleData = _scrapDataRepo.ListOfurlsNotExistingInDb(articleData);
                noOfDataToBeScrapped = filteredArticleData.Count;

                dataScrapped = await ScrapDataFromEachArtcileUrl( driver, filteredArticleData);
                var apiResponse = ResponseUtils.GetSuccesfulResponse("Data Scrapped scuccesfully and is ready for download!");
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                var responseMessage = dataScrapped + "/" + noOfDataToBeScrapped + "data scrapped successfully.Issues with Scrapping Data. Please try again for the rest!";
                var apiResponse = ResponseUtils.GetBadRequestResponse(responseMessage);
                return BadRequest(apiResponse);
            }
            finally
            {
                driver.Quit();
            }

            
        }

        [HttpPost("ScrapAllDetailedScrapInfo")]
        public async Task<IActionResult> ScrapInfoFromEachArticleUrl()
        {
           
            var driver = ChromeDriverUtils.CreateChromeDriver("https://sifted.eu/sectors");
            int noOfDataToBeScrapped = 0;
            int dataScrapped = 0;
            try
            {
                Thread.Sleep(2000);
                ChromeDriverUtils.CloseComplainaceOverlay(driver);

                ChromeDriverUtils.UserLogin(driver, _email, _password);

                Thread.Sleep(5000);
                ChromeDriverUtils.CloseComplainaceOverlay(driver);

                var articleData = _scrapDataRepo.FindSectorWiseArticleUrls();
                var filteredArticleData = _scrapDataRepo.ListOfurlsNotExistingInDb(articleData);
                noOfDataToBeScrapped = filteredArticleData.Count;

                dataScrapped = await ScrapDataFromEachArtcileUrl( driver, filteredArticleData);

                var apiResponse = ResponseUtils.GetSuccesfulResponse("Data Scrapped scuccesfully and is ready for download!");
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                var responseMessage = dataScrapped + "/" + noOfDataToBeScrapped + "data scrapped successfully.Issues with Scrapping Data. Please try again for the rest!";
                var apiResponse = ResponseUtils.GetBadRequestResponse(responseMessage);
                return BadRequest(apiResponse);
            }
            finally
            {
                driver.Quit();
            }

        }

        [HttpGet("GetAllScrapInfoAsCSV")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("text/csv")]
        public async Task<IActionResult> GetAllScrapInfoAsCSV()
        {
            try
            {
                var data = await _scrapDataRepo.FindRangeDetailedScrapDataAsync();
                return GenerateCSV(null, data);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                var apiResponse = ResponseUtils.GetBadRequestResponse(ex.Message);
                return BadRequest(apiResponse);
            }
            
        }

        [HttpGet("GetScrapInfoBySectorAsCSV")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("text/csv")]
        public async Task<IActionResult> GetScrapInfoBySectorAsCSV(string? sector = null)
        {
            try
            {
                var data = await _scrapDataRepo.FindRangeDetailedScrapDataBySectorAsync(sector);

                if (data == null)
                {
                    return BadRequest("Provide a valid Sector name");
                }

                return GenerateCSV(sector, data);
            } catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                var apiResponse = ResponseUtils.GetBadRequestResponse(ex.Message);
                return BadRequest(apiResponse);
            }

            
        }

        [HttpDelete("DeleteAllScrapOutputData")]
        public async Task<IActionResult> DeleteAllScrapOutputData()
        {
            try
            {
                await _scrapDataRepo.DeleteInitialScrapDataAsync();
                await _scrapDataRepo.DeleteSectorTableScrapDataAsync();
                await _scrapDataRepo.DeleteDetailedScrapDataAsync();
                var apiResponse = ResponseUtils.GetSuccesfulResponse("Data Deleted Successfully!");
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                var apiResponse = ResponseUtils.GetBadRequestResponse(ex.Message);
                return BadRequest(apiResponse);
            }
        }

        [HttpDelete("DeleteScrapOutputDataBySector")]
        public async Task<IActionResult> DeleteScrapOutputDataBySector(string sector)
        {
            try
            {
                await _scrapDataRepo.DeleteSectorTableScrapDataBySectorAsync(sector);
                await _scrapDataRepo.DeleteDetailedScrapDataBySectorAsync(sector);
                await _scrapDataRepo.DeleteInitialScrapDataBySectorAsync(sector);
                var apiResponse = ResponseUtils.GetSuccesfulResponse("Data Deleted Successfully!");
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                var apiResponse = ResponseUtils.GetBadRequestResponse(ex.Message);
                return BadRequest(apiResponse);

            }
        }

        [HttpGet("filtersByCategory")]
        public async Task<IActionResult> GetFiltersByCategory()
        {
            try
            {
                var categories = await _scrapDataRepo.GetFiltersByCategory();
                var apiResponse = ResponseUtils.GetSuccesfulResponse(categories);
                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                var apiResponse = ResponseUtils.GetBadRequestResponse(ex.Message);
                return BadRequest(apiResponse);

            }

        }

        [HttpGet("getSectorsToDownloadData")]
        public async Task<IActionResult> getSectorsToDownloadData()
        {
            try
            {
                var categories = await _scrapDataRepo.GetListOfSectorsToDownloadDataByCategory();
                var apiResponse = ResponseUtils.GetSuccesfulResponse(categories);
                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                var apiResponse = ResponseUtils.GetBadRequestResponse(ex.Message);
                return BadRequest(apiResponse);

            }

        }

        private async Task ScrapSectorWiseArticleUrls(ChromeDriver driver)
        {
            var ListOfInitialScrapDataFromSifted = _scrapDataRepo.FindInitialScrapOutputData();
            foreach (var outputData in ListOfInitialScrapDataFromSifted)
            {
                driver.Navigate()
                    .GoToUrl(outputData.SectorUrl);

                Thread.Sleep(2000);
                ChromeDriverUtils.CloseComplainaceOverlay(driver);
                ChromeDriverUtils.ScrollToBottmOfPage(driver);

                var listOfArticlesParentElement = driver.FIndElementByXPath("/html/body/div[2]/main/div[2]/div/ul");
                var childElement = listOfArticlesParentElement.FindAllByClass("articleListCard__link");
                var listOfArticleUrls = new List<string>();

                foreach (var article in childElement)
                {
                    var url = article.GetAttribute("href");
                    listOfArticleUrls.Add(url);

                }

                var filteredUrls = _scrapDataRepo.ListOfurlsNotExistingInSectorWiseArticleList(listOfArticleUrls);
                


                foreach (var url in filteredUrls)
                {
                    var listOfArticlesBySector = new SectorWiseArticles
                    {
                        Sectors = outputData.Sectors,
                        SectorUrl = outputData.SectorUrl,
                        ArticleUrl = url
                    };

                    await _scrapDataRepo.AddSectorWiseArticle(listOfArticlesBySector);
                }

            }
        }

        private async Task ScrapSectorsAndSectorUrls(List<InitialScrapOutputData> ListOfInitialScrapDataFromSifted, ChromeDriver driver)
        {
            var parentElement = driver.FIndElementByXPath("//*[@id=\"__next\"]/main/section/div");
            var sectors = parentElement.FindAllByTag("a");

            foreach (var sector in sectors)
            {
                var sectorUrl = sector.GetAttribute("href");
                var sectorName = sectorUrl.Split("/");
                var outputData = new InitialScrapOutputData
                {
                    Sectors = sectorName.LastOrDefault(),
                    SectorUrl = sectorUrl
                };

                if (!ListOfInitialScrapDataFromSifted.Any(o => o.SectorUrl == outputData.SectorUrl))
                {
                    ListOfInitialScrapDataFromSifted.Add(outputData);
                }

            }
            await _scrapDataRepo.AddRangeInitialScrapDataAsync(ListOfInitialScrapDataFromSifted);
        }

        private async Task<int> ScrapDataFromEachArtcileUrl(ChromeDriver driver, List<SectorWiseArticles> filteredArticleData)
        {
            int noOfDataScrapped = 0;
            foreach (var data in filteredArticleData)
            {

                driver.Navigate()
                          .GoToUrl(data.ArticleUrl);

                Thread.Sleep(2000);
                ChromeDriverUtils.CloseComplainaceOverlay(driver);

                var articleType = driver.FindElementTextByXPath("//*[@id=\"__next\"]/main/div/div[1]/div/div/div[1]/div[2]/a/p");
                var date = driver.FindElementTextByXPath("//*[@id=\"__next\"]/main/div/div[1]/div/div/div[1]/div[2]/p");
                var subject = driver.FindElementTextByXPath("//*[@id=\"__next\"]/main/div/div[1]/div/div/h1/span");
                var summary = driver.FindElementTextByXPath("//*[@id=\"__next\"]/main/div/div[1]/div/div/h2/span");
                Thread.Sleep(2000);
                ChromeDriverUtils.ScrollToBottmOfPage(driver);
                var TagsParentElement = driver.FIndElementByXPath("//*[@id=\"__next\"]/main/div/div[2]/div[3]");

                string tags;

                if (TagsParentElement == null)
                {
                    tags = "";
                }
                else
                {
                    var tagElements = TagsParentElement.FindAllByTag("a");


                    if (tagElements == null)
                    {
                        tags = "";
                    }
                    else
                    {
                        var tagList = new List<string>();
                        foreach (var item in tagElements)
                        {

                            var tag = item.FindElementTextFromParentBySelector("span > span:nth-child(2)");
                            tagList.Add(tag);
                        }
                        tags = string.Join('|', tagList.Select((x) => x).ToArray());

                    }
                }

                var scrapInfoFromArticle = new DetailedScrapOutputData
                {
                    Sector = data.Sectors,
                    Sectorurl = data.SectorUrl,
                    ContentType = articleType,
                    Date = date,
                    Subject = subject,
                    Summary = summary,
                    articleUrl = data.ArticleUrl,
                    Tags = tags
                };
                await _scrapDataRepo.AddDetailedScrapDataAsync(scrapInfoFromArticle);
                noOfDataScrapped += 1;

            }
            return noOfDataScrapped;
        }

        private IActionResult GenerateCSV(string? sector, List<DetailedScrapOutputData> data)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new(memoryStream))
                using (CsvWriter csvWriter = new(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(data);
                }

                return File(memoryStream.ToArray(), "text/csv", $"Sifted-{sector}{DateTime.Now.ToString("s")}.csv");
            }
        }


    }
}

