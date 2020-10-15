using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Opera;

using System.Drawing;
using System.Text;
using Webbott.Support;

namespace Webbott.Driver
{
    public static class SeleniumWebDriverFactory
    {
        private static BrowserSettings browserSettings;
        private static IWebDriver resultDriver;

        private const string ChromeHeadlessArgument = "--headless";
        private const string ChromeSuppressLoggingArgument = "--log-level=3";
        private const string FirefoxHeadlessArgument = "-headless";

        public static IWebDriver BuildDriver(BrowserSettings browserSettings)
        {
            SeleniumWebDriverFactory.browserSettings = browserSettings;

            switch(browserSettings.BrowserType)
            {
                case BrowserType.Chrome:
                    CreateChromeDriver();
                    break;

                case BrowserType.Firefox:
                    CreateFirefoxDriver();
                    break;

                case BrowserType.Opera:
                    CreateOperaDriver();
                    break;

                case BrowserType.Edge:
                    CreateEdgeDriver();
                    break;

                default:
                    throw InvalidBrowserTypeException.ThrowNew(browserSettings.BrowserType.Value);
            }
            ManageDriver();
            return resultDriver;
        }


        private static void CreateChromeDriver()
        {
            ChromeOptions browserOptions = new ChromeOptions();
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(WebbottConfig.BrowserDriverDirectory);
            if (browserSettings.HeadlessMode.GetValueOrDefault())
                browserOptions.AddArgument(ChromeHeadlessArgument);

            if(!browserSettings.BrowserLoggingEnabled.GetValueOrDefault())
            {
                browserOptions.AddArgument(ChromeSuppressLoggingArgument);
                driverService.SuppressInitialDiagnosticInformation = true;
            }
            if (browserSettings.EmulateMobile.GetValueOrDefault())
                browserOptions.EnableMobileEmulation("iPhone X");

            resultDriver = new ChromeDriver(driverService, browserOptions);
        }


        private static void CreateFirefoxDriver()
        {
            CodePagesEncodingProvider.Instance.GetEncoding(437);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            FirefoxOptions browserOptions = new FirefoxOptions
            {
                Profile = new FirefoxProfile(),
                BrowserExecutableLocation = WebbottConfig.FirefoxBinaryFilePath
            };
            FirefoxDriverService driverService = FirefoxDriverService.CreateDefaultService(WebbottConfig.BrowserDriverDirectory);
            if (browserSettings.HeadlessMode.GetValueOrDefault())
                browserOptions.AddArgument(FirefoxHeadlessArgument);

            if (!browserSettings.BrowserLoggingEnabled.GetValueOrDefault(true))
                driverService.SuppressInitialDiagnosticInformation = true;

            browserOptions.Profile.AcceptUntrustedCertificates = browserSettings.AllowUntrustedCertificates.GetValueOrDefault();
            resultDriver = new FirefoxDriver(driverService, browserOptions);
        }


        private static void CreateOperaDriver()
        {
            OperaOptions browserOptions = new OperaOptions { };
            OperaDriverService driverService = OperaDriverService.CreateDefaultService(WebbottConfig.BrowserDriverDirectory);
            resultDriver = new OperaDriver(driverService, browserOptions);
        }


        private static void CreateEdgeDriver()
        {
            EdgeOptions browserOptions = new EdgeOptions { };
            EdgeDriverService driverService = EdgeDriverService.CreateDefaultService(WebbottConfig.BrowserDriverDirectory);
            resultDriver = new EdgeDriver(driverService, browserOptions);
        }


        private static void ManageDriver()
        {
            if (browserSettings.Fullscreen.GetValueOrDefault())
            {
                resultDriver.Manage().Window.FullScreen();
            }
            else
            {
                var browserSize = new Size(browserSettings.ScreenWidth.GetValueOrDefault(800), browserSettings.ScreenHeight.GetValueOrDefault(600));
                resultDriver.Manage().Window.Size = browserSize;
                var browserPosition = new Point(browserSettings.ScreenPositionX.GetValueOrDefault(6), browserSettings.ScreenPositionY.GetValueOrDefault(6));
                resultDriver.Manage().Window.Position = browserPosition;
            }
        }
    }
}
