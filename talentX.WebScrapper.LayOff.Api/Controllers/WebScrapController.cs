using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium;
using talentX.WebScrapper.LayOff.Repositories.Contracts;
using talentX.WebScrapper.LayOff.Utils;
using talentX.WebScrapper.LayOff.Extensions;
using talentX.WebScrapper.LayOff.Entities;
using System.Globalization;
using CsvHelper;

namespace talentX.WebScrapper.LayOff.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebScrapController : ControllerBase
    {
        private readonly IScrapDataRepo _scrapDataRepo;

        public WebScrapController (IScrapDataRepo scrapDataRepo)
        {
            _scrapDataRepo = scrapDataRepo;
        }

        [HttpGet("LayOffScrapInfo")]
        public async Task<IActionResult> LayOffScrapInfo()
        {
            var driver = ChromeDriverUtils.CreateChromeDriver("https://layoffs.fyi/");
            var url = driver.FindElement(By.TagName("iframe")).GetAttribute("src");
            driver.Navigate().GoToUrl(url);
            await _scrapDataRepo.DeleteOutputDataAsync();


            // Deal with compliance overlay
            Thread.Sleep(2000);
            MiscUtils.CloseComplianceOverlay(driver);

            var leftPaneParentElement = driver.FindElementByClass("dataLeftPaneInnerContent");
            var rightPaneParentElement = driver.FindElementByClass("dataRightPaneInnerContent");
            int totalNoOfData = MiscUtils.FIndTotalNoOfData(driver);
            var outputDataList = new List<ScrapOutputData>();
            EventFiringWebDriver eventFiringWebDriver = new EventFiringWebDriver(driver);

            eventFiringWebDriver.Manage().Window.Maximize();
            
            var i = 0;
            var j = 0;
            while (i <= totalNoOfData)
            {
                eventFiringWebDriver.ExecuteScript($"document.querySelector('.antiscroll-inner').scrollTop={j * 400};");
                Thread.Sleep(1000);

                var leftPaneRowElements = leftPaneParentElement.FindAllByClass("dataRow");
                var rightPaneRowElements = rightPaneParentElement.FindAllByClass("dataRow");


                foreach (var leftPaneRowElement in leftPaneRowElements)
                {
                    var companyNameElement = leftPaneRowElement.FindElementTextFromParentByClass("truncate");
                    var rowNumber = leftPaneRowElement.FindElementTextFromParentByClass("numberText");

                    var rowId = leftPaneRowElement.GetAttribute("data-rowid");
                    

                    try
                    {
                        var rightPaneRowElement = rightPaneRowElements.Where((x) => x.GetAttribute("data-rowid") == rowId).FirstOrDefault();
                        var location = rightPaneRowElement.FindElementTextFromParentBySelector("div:nth-child(1) > div > span > div");
                        var laidOff = rightPaneRowElement.FindElementTextBySelectorWithChildDivElement("div:nth-child(2)");
                        var date = rightPaneRowElement.FindElementTextFromParentBySelector("div:nth-child(3)");
                        var percentage = rightPaneRowElement.FindElementTextBySelectorWithChildDivElement("div:nth-child(4)");
                        var industry = rightPaneRowElement.FindElementTextBySelectorWithChildDivElement("div:nth-child(5)");
                        var source = rightPaneRowElement.FindElementTextBySelectorWithChildDivElement("div:nth-child(6)");
                        var employees = rightPaneRowElement.FindElementTextBySelectorWithChildDivElement("div:nth-child(7)");
                        var stage = rightPaneRowElement.FindElementTextBySelectorWithChildDivElement("div:nth-child(8)");
                        var raised = rightPaneRowElement.FindElementTextFromParentBySelector("div:nth-child(9)");
                        var country = rightPaneRowElement.FindElementTextFromParentBySelector("div:nth-child(10)");
                        var dateAdded = rightPaneRowElement.FindElementTextFromParentBySelector("div:nth-child(11)");

                        if (!string.IsNullOrWhiteSpace(rowId) && !string.IsNullOrWhiteSpace(location) && !string.IsNullOrWhiteSpace(companyNameElement))
                        {
                            var info = new ScrapOutputData
                            {
                                elementName = rowId,
                                numberText = rowNumber,
                                CompanyName = companyNameElement,
                                LocationHQ = location,
                                LaidOff = laidOff,
                                Date = date,
                                Percentage = percentage,
                                Industry = industry,
                                SourceUrl = source,
                                listOfLaidOffEmployeesUrl = employees,
                                Stage = stage,
                                Raised = raised,
                                Country = country,
                                DateAdded = dateAdded
                            };

                            if (!outputDataList.Any(o => o.elementName == info.elementName))
                            {
                                outputDataList.Add(info);
                            }

                        }
                       
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Unable to retrive info");
                    }

                }
                var ids = leftPaneParentElement.FindAllByClass("numberText");
                var validIds = ids.Where((x) => !string.IsNullOrWhiteSpace(x.Text)).ToList();
                


                i = int.Parse(validIds.LastOrDefault().Text) + 1;
                Console.WriteLine(i);
                j++;
            }


            await _scrapDataRepo.AddRangeOutputDataAsync(outputDataList);
            driver.Quit();
            return Ok();
        }

        [HttpGet("GetScrapInfoAsCSV")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("text/csv")]
        public async Task<IActionResult> GetScrapInfoAsCSV()
        {
            var data = await _scrapDataRepo.FindOutputDataAsync();

            using (var memoryStream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new(memoryStream))
                using (CsvWriter csvWriter = new(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(data);
                }

                return File(memoryStream.ToArray(), "text/csv", $"LayOffScrapper-{DateTime.Now.ToString("s")}.csv");
            }
        }
    }
}
