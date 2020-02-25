using System;

namespace Metrics
{
    public interface IMetric : IComparable<IMetric>
    {
        MetricName Name { get; }
    }
}