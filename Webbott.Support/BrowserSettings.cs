using System;

namespace Webbott.Support
{
    public class BrowserSettings
    {
        public BrowserType? BrowserType { get; set; }
        public bool? HeadlessMode { get; set; }
        public bool? BrowserLoggingEnabled { get; set; }
        public int? ScreenWidth { get; set; }
        public int? ScreenHeight { get; set; }
        public int? ScreenPositionX { get; protected set; }
        public int? ScreenPositionY { get; protected set; }
        public bool? AllowUntrustedCertificates { get; set; }
        public bool? EmulateMobile { get; set; }
        public bool? Fullscreen { get; set; }
        public int? Timeout { get; set; }
        public int? MaximumClickAttempts { get; set; }
        public int? RandomSeed { get; set; }


        public static BrowserSettings DefaultSettings
            => new BrowserSettings
            {
                BrowserType = Webbott.Support.BrowserType.Chrome,
                HeadlessMode = false,
                BrowserLoggingEnabled = true,
                AllowUntrustedCertificates = false,
                Fullscreen = false,
                ScreenWidth = 900,
                ScreenHeight = 600,
                ScreenPositionX = 10,
                ScreenPositionY = 10,
                EmulateMobile = false,
                RandomSeed = 
                    3600000 * DateTime.Now.Hour +
                    60000 * DateTime.Now.Minute + 
                    1000 * DateTime.Now.Second + 
                    DateTime.Now.Millisecond,
                Timeout = SharedConstants.DefaultTimeoutMilliseconds,
                MaximumClickAttempts = 10
            };


        public void OverrideSettingsWith(BrowserSettings overrideSettings)
        {
            if (overrideSettings == null)
                return;

            BrowserType = overrideSettings.BrowserType ?? BrowserType;
            HeadlessMode = overrideSettings.HeadlessMode ?? HeadlessMode;
            BrowserLoggingEnabled = overrideSettings.BrowserLoggingEnabled ?? BrowserLoggingEnabled;
            ScreenWidth = overrideSettings.ScreenWidth ?? ScreenWidth;
            ScreenHeight = overrideSettings.ScreenHeight ?? ScreenHeight;
            ScreenPositionX = overrideSettings.ScreenPositionX ?? ScreenPositionX;
            ScreenPositionY = overrideSettings.ScreenPositionY ?? ScreenPositionY;
            EmulateMobile = overrideSettings.EmulateMobile ?? EmulateMobile;
            Timeout = overrideSettings.Timeout ?? Timeout;
            RandomSeed = overrideSettings.RandomSeed ?? RandomSeed;
            MaximumClickAttempts = overrideSettings.MaximumClickAttempts ?? MaximumClickAttempts;
        }


        public void MergeSettingsWith(BrowserSettings mergeSettings)
        {
            if (mergeSettings == null)
                return;

            BrowserType ??= mergeSettings.BrowserType;
            HeadlessMode ??= mergeSettings.HeadlessMode;
            BrowserLoggingEnabled ??= mergeSettings.BrowserLoggingEnabled;
            ScreenWidth ??= mergeSettings.ScreenWidth;
            ScreenHeight ??= mergeSettings.ScreenHeight;
            ScreenPositionX ??= mergeSettings.ScreenPositionX;
            ScreenPositionY ??= mergeSettings.ScreenPositionY;
            EmulateMobile ??= mergeSettings.EmulateMobile;
            Timeout ??= mergeSettings.Timeout;
            RandomSeed ??= mergeSettings.RandomSeed;
            MaximumClickAttempts ??= mergeSettings.MaximumClickAttempts;
        }


        public void SetBrowser(BrowserType browserType)
            => BrowserType = browserType;

        public void SetScreenPosition(int x, int y)
        {
            if (x > 0)
                ScreenPositionX = x;

            if (y > 0)
                ScreenPositionY = y;
        }


        public void SetScreenSize(int width, int height)
        {
            if (width > 0)
                ScreenWidth = width;

            if (height > 0)
                ScreenHeight = height;
        }
    }
}
