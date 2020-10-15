using System;
using System.Globalization;
using System.IO;

namespace Webbott.Support
{
    public static class SupportMethods
    {
        public static string UniqueId
        {
            get
            {
                double totalDays = (DateTime.Now - SharedConstants.BeginningOfTime).TotalDays;
                string idSeed = totalDays.ToString(CultureInfo.InvariantCulture);
                int separatorIndex = idSeed.IndexOf('.');
                idSeed = $"{idSeed.Substring(separatorIndex - 4, 4)}{idSeed.Substring(separatorIndex + 4, 4)}";
                int idSeedInt = int.Parse(idSeed, CultureInfo.InvariantCulture);
                return Base10ToBase36(idSeedInt);
            }
        }


        public static string CurrentTimeAsString => DateTime.Now.ToString("yyyyMMddHHmmssfff");


        public static string Base10ToBase36(int base10)
        {
            string result = "";
            while(base10 != 0)
            {
                result += SharedConstants.Base36Digits[base10 % 36];
                base10 /= 36;
            }
            return result;
        }


        public static string ExtractDigits(string input)
        {
            string digits = "0123456789";
            string result = "";
            for(int i = 0; i < input.Length; i++)
                if (digits.Contains(input[i]))
                    result += input[i];

            return result;
        }


        public static bool PromptYesNo(string message)
        {
            Console.WriteLine($"{message} Y/n");
            return Console.ReadLine().Equals("Y", StringComparison.InvariantCultureIgnoreCase);
        }


        public static void BuildDirectories()
        {
            BuildFullPath(WebbottConfig.ScreenshotDirectory);
            BuildFullPath(WebbottConfig.BrowserDriverDirectory);
            BuildFullPath(WebbottConfig.ScreenshotDirectory);
        }


        private static void BuildFullPath(string path)
        {
            path = Path.GetFullPath(path);
            Directory.CreateDirectory(path);
        }
    }
}
