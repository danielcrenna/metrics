using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Metrics.Reporters.Console;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Metrics.Reporters.Logging
{
    public sealed class LogReporter : PeriodicReporter
    {
        private readonly ILogger _logger;
        private readonly IOptions<LogReporterOptions> _options;
        private readonly IMetricsRegistry _registry;

        public LogReporter(IMetricsRegistry registry, ILoggerFactory loggerFactory,
            IOptions<LogReporterOptions> options) : base(options.Value.Interval)
        {
            _registry = registry;
            _options = options;
            _logger = loggerFactory.CreateLogger(options.Value.CategoryName);
        }

        public override Task Report(CancellationToken cancellationToken = default)
        {
            if (_logger == null || cancellationToken.IsCancellationRequested) return Task.CompletedTask;

            try
            {
                using var stream = new MemoryStream();

                var encoding = Encoding.UTF8;
                using (var writer = new StreamWriter(stream, encoding))
                {
                    if (!ConsoleReporter.TryWrite(_registry, writer, cancellationToken) &&
                        _options.Value.StopOnError)
                    {
                        Stop();
                        return Task.CompletedTask;
                    }
                }

                _logger.Log(_options.Value.LogLevel, encoding.GetString(stream.ToArray()));
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Error reporting metrics to logger");
                if (_options.Value.StopOnError) Stop();
            }

            return Task.CompletedTask;
        }
    }
}