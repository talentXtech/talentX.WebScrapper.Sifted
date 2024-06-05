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

        public WebScrapController(IScrapDataRepo scrapDataRepo)
        {
            _scrapDataRepo = scrapDataRepo;
        }

        [HttpGet("SiftedScrapInfo")]
        public async Task<ActionResult> SiftedScrapInfo(string email = "abi1243@gmail.com", string password = "Sifted1234!")
        {
            var ListOfInitialScrapDataFromSifted = new List<InitialScrapOutputData>();
            var driver = ChromeDriverUtils.CreateChromeDriver("https://sifted.eu/sectors");
            try
            {


                Thread.Sleep(2000);
                ChromeDriverUtils.CloseComplainaceOverlay(driver);

                ChromeDriverUtils.UserLogin(driver, email, password);

                Thread.Sleep(5000);
                ChromeDriverUtils.CloseComplainaceOverlay(driver);

                ChromeDriverUtils.ScrollToBottmOfPage(driver);
                Thread.Sleep(2000);

                await ScrapSectorsAndSectorUrls(ListOfInitialScrapDataFromSifted, driver);

                await ScrapSectorWiseArticleUrls(driver);


                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return BadRequest(ex.Message);
            }
            finally
            {
                driver.Quit();
            }


        }

        [HttpGet("SiftedDetailedScrapInfo")]
        public async Task<IActionResult> ScrapInfoFromEachArticleUrl(string email = "abi1243@gmail.com", string password = "Sifted1234!", string? sector = null)
        {
            var ListOfFinalScrappedDataFromSifted = new List<DetailedScrapOutputData>();
            var driver = ChromeDriverUtils.CreateChromeDriver("https://sifted.eu/sectors");
            try
            {
                Thread.Sleep(2000);
                ChromeDriverUtils.CloseComplainaceOverlay(driver);

                ChromeDriverUtils.UserLogin(driver, email, password);

                Thread.Sleep(5000);
                ChromeDriverUtils.CloseComplainaceOverlay(driver);

                var articleData = _scrapDataRepo.FindSectorWiseArticleUrls(sector);


                foreach (var data in articleData)
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
                    } else
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
                    ListOfFinalScrappedDataFromSifted.Add(scrapInfoFromArticle);

                }
                await _scrapDataRepo.AddRangeDetailedScrapDataAsync(ListOfFinalScrappedDataFromSifted);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return BadRequest(ex.Message);
            }
            finally
            {
                driver.Quit();
            }

        }

        [HttpGet("GetScrapInfoAsCSV")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("text/csv")]
        public async Task<IActionResult> GetScrapInfoAsCSV(string? sector = null)
        {
            var data = await _scrapDataRepo.FindRangeDetailedScrapDataAsync(sector);

            if (data == null)
            {
                return BadRequest("Provide a valid Sector name");
            }

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

        [HttpDelete("DeleteInitialScrapOutputData")]
        public async Task<IActionResult> DeleteInitialScrapOutputData()
        {
            try
            {
                await _scrapDataRepo.DeleteInitialScrapDataAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);

            }
        }

        [HttpDelete("DeleteDetailedScrapOutputData")]
        public async Task<IActionResult> DeleteDetailedScrapOutputData()
        {
            try
            {
                await _scrapDataRepo.DeleteDetailedScrapDataAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);

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

                var filteredUrls = _scrapDataRepo.ListOfurlsNotExistingInDb(listOfArticleUrls);
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

        
    }
}

