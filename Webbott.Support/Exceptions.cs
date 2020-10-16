using System;

namespace Webbott.Support
{
    public class MissingConfigurationKeyException : Exception
    {
        public MissingConfigurationKeyException(string message) : base(message) { }

        public static MissingConfigurationKeyException ThrowNew(string configurationKey)
        {
            string message = $"Key {configurationKey} not found in configuration file";
            return new MissingConfigurationKeyException(message);
        }
    }


    public class InvalidBrowserTypeException : Exception
    {
        public InvalidBrowserTypeException(string message) : base(message) { }

        public static InvalidBrowserTypeException ThrowNew(BrowserType browserType)
        {
            string message = $"Browser {browserType} is not supported";
            return new InvalidBrowserTypeException(message);
        }
    }


    public class InvalidTimeUnitException : Exception
    {
        public InvalidTimeUnitException(string message) : base(message) { }

        public static InvalidTimeUnitException ThrowNew(TimeUnit timeUnit)
        {
            string message = $"Time Unit {timeUnit} not supported";
            return new InvalidTimeUnitException(message);
        }
    }


    public class UnknownSelectorTypeException : Exception
    {
        public UnknownSelectorTypeException(string message) : base(message) { }


        public static UnknownSelectorTypeException ThrowNew(Type type)
        {
            string message = $"The selector type {type.Name} was not a recognized type";
            return new UnknownSelectorTypeException(message);
        }
    }


    public class WaitTimeoutException : Exception
    {
        public WaitTimeoutException(string message) : base(message) { }


        public static WaitTimeoutException ThrowNew()
        {
            string message = $"The operation timed out";
            return new WaitTimeoutException(message);
        }
    }


    public class ClickAttemptsExceededException : Exception
    {
        public ClickAttemptsExceededException(string message) : base(message) { }


        public static ClickAttemptsExceededException ThrowNew()
        {
            string message = $"The operation exceeded the amount of allowed click attempts";
            return new ClickAttemptsExceededException(message);
        }
    }
}
