using System;

namespace Metrics.Reporters.Console
{
    public class ConsoleReporterOptions
    {
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(5);
        public bool StopOnError { get; set; } = false;
    }
}