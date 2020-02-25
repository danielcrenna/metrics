using System;

namespace Metrics
{
    public interface IMetricsHost<out TOwner> : IReadableMetrics
    {
        GaugeMetric<T> Gauge<T>(string name, Func<T> evaluator);
        CounterMetric Counter(string name);
        HistogramMetric Histogram(string name, SampleType sampleType);
        MeterMetric Meter(string name, string eventType, TimeUnit rateUnit);
        TimerMetric Timer(string name, TimeUnit durationUnit, TimeUnit rateUnit);
        void Clear();
    }
}