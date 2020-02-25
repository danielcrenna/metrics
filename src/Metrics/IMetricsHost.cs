using System;

namespace Metrics
{
    public interface IMetricsHost : IReadableMetrics
    {
        bool Clear();

        #region Default Owner

        GaugeMetric<T> Gauge<T>(string name, Func<T> evaluator);
        CounterMetric Counter(string name);
        HistogramMetric Histogram(string name, SampleType sampleType);
        MeterMetric Meter(string name, string eventType, TimeUnit rateUnit);
        TimerMetric Timer(string name, TimeUnit durationUnit, TimeUnit rateUnit);

        #endregion

        #region Owner

        GaugeMetric<T> Gauge<T>(Type owner, string name, Func<T> evaluator);
        CounterMetric Counter(Type owner, string name);
        HistogramMetric Histogram(Type owner, string name, SampleType sampleType);
        MeterMetric Meter(Type owner, string name, string eventType, TimeUnit rateUnit);
        TimerMetric Timer(Type owner, string name, TimeUnit durationUnit, TimeUnit rateUnit);

        #endregion
    }
}