using Microsoft.Extensions.Configuration;
using System.IO;

namespace Webbott.Support
{
    public static class WebbottConfig
    {
        private const string ConfigKeyScreenshotDirectory = "ScreenshotDirectory";
        private const string ConfigKeyBrowserDriverDirectory = "BrowserDriverDirectory";
        private const string ConfigKeyFirefoxBinaryFilePath = "FirefoxBinaryFilePath";

        private static IConfiguration Configuration
        {
            get
            {
                configurationInstance ??= new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true)
                    .Build();
                return configurationInstance;
            }
        }
        private static IConfiguration configurationInstance;


        public static string ScreenshotDirectory
        {
            get
            {
                string result = GetMandatoryKey(ConfigKeyScreenshotDirectory);
                return Path.GetFullPath(result);
            }
        }

        public static string BrowserDriverDirectory
        {
            get
            {
                string result = GetMandatoryKey(ConfigKeyBrowserDriverDirectory);
                return Path.GetFullPath(result);
            }
        }


        public static string FirefoxBinaryFilePath
        {
            get
            {
                string result = GetMandatoryKey(ConfigKeyFirefoxBinaryFilePath);
                return Path.GetFullPath(result);
            }
        }


        public static string GetKeyOrDefault(string keyName, string defaultValue = null)
            => Configuration[keyName] ?? defaultValue;


        public static string GetMandatoryKey(string keyName)
        {
            string result = GetKeyOrDefault(keyName, null);
            if (result == null)
                throw MissingConfigurationKeyException.ThrowNew(keyName);

            return result;
        }
    }
}
