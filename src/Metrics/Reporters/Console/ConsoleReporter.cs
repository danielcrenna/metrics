using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Metrics.Internal;
using Microsoft.Extensions.Options;

namespace Metrics.Reporters.Console
{
    /// <summary>
    ///     A simple reporter which prints out application metrics to a <see cref="TextWriter" /> periodically.
    /// </summary>
    public sealed class ConsoleReporter : PeriodicReporter
    {
        private readonly IOptions<ConsoleReporterOptions> _options;
        private readonly TextWriter _out;
        private readonly IMetricsRegistry _registry;

        public ConsoleReporter(IMetricsRegistry registry, IOptions<ConsoleReporterOptions> options) : this(
            System.Console.Out, registry, options)
        {
        }

        private ConsoleReporter(TextWriter @out, IMetricsRegistry registry, IOptions<ConsoleReporterOptions> options) :
            base(options.Value.Interval)
        {
            _out = @out;
            _registry = registry;
            _options = options;
        }


        public override Task Report(CancellationToken cancellationToken = default)
        {
            if (!TryWrite(_registry, _out, cancellationToken) && _options.Value.StopOnError) Stop();

            return Task.CompletedTask;
        }

        public static bool TryWrite(IMetricsRegistry registry, TextWriter @out, CancellationToken? cancellationToken)
        {
            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested) return true;

            try
            {
                cancellationToken?.ThrowIfCancellationRequested();

                var now = DateTime.Now;
                var dateTime = $"{now.ToShortDateString()} {now.ToShortTimeString()}";
                @out.Write(dateTime);
                @out.Write(' ');
                for (var i = 0; i < 80 - dateTime.Length - 1; i++) @out.Write('=');

                @out.WriteLine();

                foreach (var host in registry)
                foreach (var entry in host.GetSample().Sort())
                {
                    @out.Write(entry.Key);
                    @out.WriteLine(':');

                    foreach (var subEntry in entry.Value)
                    {
                        @out.Write("  ");
                        @out.Write(subEntry.Key);
                        @out.WriteLine(':');

                        var metric = subEntry.Value;
                        switch (metric)
                        {
                            case GaugeMetric _:
                                WriteGauge(@out, (GaugeMetric) metric);
                                break;
                            case CounterMetric _:
                                WriteCounter(@out, (CounterMetric) metric);
                                break;
                            case HistogramMetric _:
                                WriteDistributed(@out, (HistogramMetric) metric);
                                break;
                            case MeterMetric _:
                                WriteMetered(@out, (MeterMetric) metric);
                                break;
                            case TimerMetric _:
                                WriteTimer(@out, (TimerMetric) metric);
                                break;
                        }

                        @out.WriteLine();
                    }

                    @out.WriteLine();
                    @out.Flush();
                }

                return true;
            }
            catch (Exception e)
            {
                @out.WriteLine(e.StackTrace);
                return false;
            }
        }

        private static void WriteGauge(TextWriter writer, GaugeMetric gauge)
        {
            writer.Write("    value = ");
            writer.WriteLine(gauge.ValueAsString);
        }

        private static void WriteCounter(TextWriter writer, CounterMetric counter)
        {
            writer.Write("    count = ");
            writer.WriteLine(counter.Count);
        }

        private static void WriteMetered(TextWriter writer, IMetered meter)
        {
            var unit = meter.RateUnit.Abbreviate();
            writer.Write("             count = {0}\n", meter.Count);
            writer.Write("         mean rate = {0} {1}/{2}\n", meter.MeanRate, meter.EventType, unit);
            writer.Write("     1-minute rate = {0} {1}/{2}\n", meter.OneMinuteRate, meter.EventType, unit);
            writer.Write("     5-minute rate = {0} {1}/{2}\n", meter.FiveMinuteRate, meter.EventType, unit);
            writer.Write("    15-minute rate = {0} {1}/{2}\n", meter.FifteenMinuteRate, meter.EventType, unit);
        }

        private static void WriteDistributed(TextWriter writer, IDistributed distribution)
        {
            var percentiles = distribution.Percentiles(0.5, 0.75, 0.95, 0.98, 0.99, 0.999);

            writer.Write("               min = %{0:2}\n", distribution.Min);
            writer.Write("               max = %{0:2}\n", distribution.Max);
            writer.Write("              mean = %{0:2}\n", distribution.Mean);
            writer.Write("            stddev = %{0:2}\n", distribution.StdDev);
            writer.Write("            median = %{0:2}\n", percentiles[0]);
            writer.Write("              75%% <= %{0:2}\n", percentiles[1]);
            writer.Write("              95%% <= %{0:2}\n", percentiles[2]);
            writer.Write("              98%% <= %{0:2}\n", percentiles[3]);
            writer.Write("              99%% <= %{0:2}\n", percentiles[4]);
            writer.Write("            99.9%% <= %{0:2}\n", percentiles[5]);
        }

        private static void WriteTimer(TextWriter writer, TimerMetric timer)
        {
            WriteMetered(writer, timer);

            WriteDistributed(writer, timer);
        }
    }
}