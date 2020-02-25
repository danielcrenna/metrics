using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Metrics
{
    public class InMemoryMetricsStore : InMemoryKeyValueStore<MetricName, IMetric>, IMetricsStore
    {
        private static readonly IImmutableDictionary<MetricName, IMetric> NoSample =
            ImmutableDictionary.Create<MetricName, IMetric>();

        public IImmutableDictionary<MetricName, IMetric> GetSample(MetricType typeFilter = MetricType.All)
        {
            if (typeFilter.HasFlagFast(MetricType.All)) return NoSample;

            var filtered = new Dictionary<MetricName, IMetric>();
            foreach (var entry in Memory)
                switch (entry.Value)
                {
                    case GaugeMetric _ when typeFilter.HasFlagFast(MetricType.Gauge):
                    case CounterMetric _ when typeFilter.HasFlagFast(MetricType.Counter):
                    case MeterMetric _ when typeFilter.HasFlagFast(MetricType.Meter):
                    case HistogramMetric _ when typeFilter.HasFlagFast(MetricType.Histogram):
                    case TimerMetric _ when typeFilter.HasFlagFast(MetricType.Timer):
                        continue;

                    default:
                        filtered.Add(entry.Key, entry.Value);
                        break;
                }

            return filtered.ToImmutableSortedDictionary(k => k.Key, v =>
            {
                return v.Value switch
                {
                    GaugeMetric gauge => gauge.Copy(),
                    CounterMetric counter => counter.Copy(),
                    MeterMetric meter => meter.Copy(),
                    HistogramMetric histogram => histogram.Copy(),
                    TimerMetric timer => timer.Copy(),
                    _ => throw new ArgumentException()
                };
            });
        }
    }
}