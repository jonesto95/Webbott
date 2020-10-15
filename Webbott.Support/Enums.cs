namespace Webbott.Support
{
    public enum LauncherCallerSource
    {
        Console,
        FileStructure
    }

    public enum ExecutionMode
    {
        Launcher,
        TestCase,
        Plugin,
        Script
    }

    public enum BrowserType
    {
        Chrome,
        Firefox,
        Opera,
        Edge
    }

    public enum LoggingLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL
    }

    public enum DataType
    {
        Table = 1,
        NULL = 31,
        Image = 34,
        Text = 35,
        UniqueIdentifier = 36,
        Date = 40,
        Time = 41,
        DateTime2 = 42,
        DateTimeOffset = 43,
        TinyInt = 48,
        SmallInt = 52,
        Integer = 56,
        SmallDateTime = 58,
        Real = 59,
        Money = 60,
        DateTime = 61,
        Float = 62,
        Sql_Variant = 98,
        Ntext = 99,
        Bit = 104,
        Decimal = 106,
        Numeric = 108,
        SmallMoney = 122,
        Bigint = 127,
        HeirarchyId = 128,
        Geometry = 129,
        Geography = 130,
        VarBinary = 165,
        VarChar = 167,
        Binary = 173,
        Char = 175,
        Timestamp = 189,
        NVarChar = 231,
        Nchar = 239,
        Xml = 241,
        Xml2 = 242,
        Table_Type = 243,
        Cursor = 250,
        SysName = 256
    }

    public enum SchemaValidationResponse
    {
        Complete,
        InProgress,
        CouldNotConnect,
        Incomplete,
        Nonexistent,
        Error
    }


    public enum TimeUnit
    {
        Milliseconds,
        Seconds,
        Minutes,
        Hours,
        Days,
        Weeks
    }
}
