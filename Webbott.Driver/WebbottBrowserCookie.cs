using OpenQA.Selenium;

using System;

namespace Webbott.Driver
{
    public class WebbottBrowserCookie
    {
        public string Domain { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Value { get; set; }
        public bool IsHttpOnly { get; }
        public bool IsSecure { get; }
        public DateTime? ExpirationDate { get; set; }


        public WebbottBrowserCookie(string name, string value, DateTime? expirationDate, string path, string domain)
        {
            Name = name;
            Domain = domain;
            ExpirationDate = expirationDate;
            Path = path;
            Value = value;
        }


        public static WebbottBrowserCookie FromSeleniumCookie(Cookie seleniumCookie)
            => new WebbottBrowserCookie(
                seleniumCookie.Name,
                seleniumCookie.Value,
                seleniumCookie.Expiry,
                seleniumCookie.Path,
                seleniumCookie.Domain);


        public Cookie ToSeleniumCookie()
            => new Cookie(Name, Value, Domain, Path, ExpirationDate);
    }
}
