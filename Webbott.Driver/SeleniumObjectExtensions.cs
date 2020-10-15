using OpenQA.Selenium;

namespace Webbott.Driver
{
    public static class SeleniumObjectExtensions
    {
        public static bool IsActive(this IWebElement domElement)
            => domElement.Enabled && domElement.Displayed;
    }
}
