using System.Collections.Immutable;

namespace Metrics
{
    public interface IMetricsStore<TFilter> : IKeyValueStore<MetricName, TFilter> where TFilter : IMetric
    {
        IImmutableDictionary<MetricName, TFilter> GetSample(MetricType typeFilter = MetricType.None);
    }
}