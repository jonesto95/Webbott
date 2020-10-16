using System;
using System.Threading;

namespace Webbott.Support
{
    public static class Timer
    {
        public static double MillisecondsElapsed
            => GetTimeElapsed(TimeUnit.Milliseconds);

        public static double SecondsElapsed
            => GetTimeElapsed(TimeUnit.Seconds);

        public static double? TimeoutSeconds { get; set; }

        private static DateTime? StartTime { get; set; }

        private static bool TimerStarted { get; set; } = false;


        public static void StartTimer()
        {
            if (TimerStarted)
                throw new TimerAlreadyStartedException();

            TimerStarted = true;
            StartTime = DateTime.UtcNow;
            Thread t = new Thread(TimeoutCheck);
            t.Start();
        }


        public static void TryStartTimer()
        {
            TimerStarted = true;
            StartTime = DateTime.UtcNow;
            Thread t = new Thread(TimeoutCheck);
            t.Start();
        }


        public static double EndTimer()
            => EndTimer(TimeUnit.Seconds);


        public static double EndTimer(TimeUnit timeUnit)
        {
            double result = GetTimeElapsed(timeUnit);
            if (!TimerStarted)
                throw new TimerNotStartedException();

            TimerStarted = false;
            StartTime = null;
            return result;
        }


        public static void ForceEndTimer()
        {
            TimerStarted = false;
            StartTime = null;
        }


        private static double GetTimeElapsed(TimeUnit timeUnit)
        {
            if (!TimerStarted)
                return -1;

            var now = DateTime.UtcNow;
            var timeSpan = now - StartTime.Value;

            return timeUnit switch
            {
                TimeUnit.Milliseconds => timeSpan.TotalMilliseconds,
                TimeUnit.Seconds => timeSpan.TotalSeconds,
                TimeUnit.Minutes => timeSpan.TotalMinutes,
                TimeUnit.Hours => timeSpan.TotalHours,
                TimeUnit.Days => timeSpan.Days,
                TimeUnit.Weeks => 7 * timeSpan.Days,
                _ => throw InvalidTimeUnitException.ThrowNew(timeUnit),
            };
        }


        private static void TimeoutCheck(object _)
        {
            while(TimerStarted)
            {
                if(TimeoutSeconds.HasValue)
                {
                    double timeElapsed = GetTimeElapsed(TimeUnit.Seconds);
                    if (timeElapsed > TimeoutSeconds.Value)
                        throw new TimeoutException();
                }
            }
        }
    }


    internal class TimerAlreadyStartedException : Exception
    {
        public TimerAlreadyStartedException() : base("The timer has already been started") { }
    }


    internal class TimerNotStartedException : Exception
    {
        public TimerNotStartedException() : base("The timer has not been started") { }
    }


    internal class TimerTimeoutException : Exception
    {
        public TimerTimeoutException() : base("Timer has exceeded the allowed time limit") { }
    }
}
