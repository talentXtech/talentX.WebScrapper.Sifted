using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using talentX.WebScrapper.Sifted.Extensions;

namespace talentX.WebScrapper.Sifted.Utils
{
    public class ChromeDriverUtils
    {
        public static ChromeDriver CreateChromeDriver(string url)
        {
            var options = new ChromeOptions();
            options.AddArguments("--ignore-ssl-errors", "--verbose", "--disable-dev-shm-usage");
            //        "--headless",
            //        "--verbose",
            //        "--disable-dev-shm-usage"
            var driver = new ChromeDriver(options);


            driver.Navigate().GoToUrl(url);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
            driver.Manage().Window.Maximize();

            return driver;
        }

        public static ChromeDriver CreateChromeDriverHeadless(string url)
        {
            var options = new ChromeOptions();
            options.AddArguments("--ignore-ssl-errors", "--headless", "--verbose", "--disable-dev-shm-usage");
            //        "--headless",
            //        "--verbose",
            //        "--disable-dev-shm-usage"
            var driver = new ChromeDriver(options);


            driver.Navigate().GoToUrl(url);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
            driver.Manage().Window.Maximize();


            return driver;
        }

        public static void ScrollToBottmOfPage(ChromeDriver driver, int sleepMs = 2000)
        {
            // Keep track of the last height
            var lastHeight = (long)driver.ExecuteScript("return document.body.scrollHeight");

            while (true)
            {
                // Scroll down to the bottom of the page
                driver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

                // Wait for the new content to load
                Thread.Sleep(sleepMs);

                // Check the new scroll height and compare it with the last scroll height
                var newHeight = (long)driver.ExecuteScript("return document.body.scrollHeight");

                if (newHeight == lastHeight)
                {
                    // End of page, break the loop
                    break;
                }
                lastHeight = newHeight;
            }
        }

        public static void CloseComplainaceOverlay(ChromeDriver driver)
        {
            var complianceOverlay = driver.FindElements(By.Id("CybotCookiebotDialogBodyButtonsWrapper"));
            if (complianceOverlay.Count > 0)
            {
                complianceOverlay[0].ClickButtonById("CybotCookiebotDialogBodyLevelButtonLevelOptinAllowAll");
            }
        }

        public static void UserLogin(ChromeDriver driver, string email, string password)
        {
            driver.ClickButtonByClass("ga-nav-link-login");
            Thread.Sleep(2000);

            var loginEmailInput = driver.FindElement(By.Id("email"));
            CloseComplainaceOverlay(driver);
            loginEmailInput.Clear();
            loginEmailInput.SendKeys(email);
            CloseComplainaceOverlay(driver);

            var continueButton = driver.FindElement(By.XPath("/html/body/div[2]/div/main/div/div/div[2]/div/form/div/div[2]/div/button"));
            continueButton.Click();

            Thread.Sleep(5000);
            var passwordInput = driver.FindElement(By.Id("password"));
            passwordInput.Clear();
            passwordInput.SendKeys(password);

            var continueButton2 = driver.FindElement(By.XPath("/html/body/div[2]/div/main/div/div/div[2]/div/form/div/div[4]/div/button"));
            continueButton2.Click();
        }
    }
}