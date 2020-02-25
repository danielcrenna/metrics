using System;
using Microsoft.Extensions.Logging;

namespace Metrics.Reporters.Logging
{
    public class LogReporterOptions
    {
        public string CategoryName { get; set; } = Constants.Categories.Metrics;
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(5);
        public bool StopOnError { get; set; } = false;
    }
}