using System;

namespace Webbott.Support
{
    public class SharedConstants
    {
        internal const string Base36Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public const string ConsoleFunctionLauncher = "/launcher";
        public const string ConsoleFunctionTestCase = "/testcase";
        public const string ConsoleFunctionPlugin = "/plugin";
        public const string ConsoleFunctionScript = "/script";

        public const string ConsoleSettingsDefineBrowser = "-browser";
        public const string ConsoleSettingsSetThreadCount = "-threadcount";
        public const string ConsoleSettingsEnableDebugMode = "-debug";
        public const string ConsoleSettingsEnableHeadlessMode = "-headless";
        public const string ConsoleSettingsEnableFullscreen = "-fullscreen";
        public const string ConsoleSettingsSuppressBrowserLogging = "-nobrowserlog";
        public const string ConsoleSettingsSetScreenSize = "-screensize";
        public const string ConsoleSettingsSetScreenPosition = "-screenposition";
        public const string ConsoleSettingsEmulateMobile = "-mobile";
        public const string ConsoleSettingsAllowAllCerts = "-allcerts";
        public const string ConsoleSettingsNoDatabaseWriting = "-nodb";
        public const string ConsoleSettingsSetTestCategory = "-category";
        public const string ConsoleSettingsSetTestCaseIds = "-testcases";
        public const string ConsoleSettingsSetSourceFileName = "-filename";
        public const string ConsoleSettingsSetClassFile = "-class";
        public const string ConsoleSettingsSetInitialVariables = "-values";
        public const string ConsoleSettingsNoDatabaseValidation = "-nodbval";
        public const string ConsoleSettingsSetBrowserConfigSource = "-browsersource";
        public const string ConsoleSettingsSetBrowserTimeout = "-timeout";
        public const string ConsoleSettingsMaximumClickAttempts = "-maxclicks";
        public const string ConsoleSettingsBrowserFromLauncher = "console";
        public const string ConsoleSettingsBrowserFromScript = "script";

        public const string DebugThreadId = "D.BUG";
        public const string FormKeySkipValue = "[skip]";
        public const string FormValueFormat = "{0}_{1}";
        public const string GenericErrorOccurredMessage = "An error has occurred: ";
        public static readonly DateTime BeginningOfTime = DateTime.Parse("1/1/1900");

        public const string DatabaseDefaultGuidStatement = "dbo.NewWebbottGuid(NEWID())";
        public const string DatabaseGetUtcTimeStatement = "GETUTCTIME()";
        public const int DefaultTimeoutMilliseconds = 60000;
    }
}
