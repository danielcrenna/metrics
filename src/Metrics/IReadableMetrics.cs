using System.Collections.Immutable;

namespace Metrics
{
    public interface IReadableMetrics
    {
        IImmutableDictionary<MetricName, IMetric> GetSample(MetricType typeFilter = MetricType.None);
    }
}