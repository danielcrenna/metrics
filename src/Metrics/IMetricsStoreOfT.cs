using System.Collections.Immutable;
using ActiveStorage;

namespace Metrics
{
    public interface IMetricsStore<TFilter> : IKeyValueStore<MetricName, TFilter>, ICanClear where TFilter : IMetric
    {
        IImmutableDictionary<MetricName, TFilter> GetSample(MetricType typeFilter = MetricType.None);
    }
}