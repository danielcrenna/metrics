using System.Collections.Generic;

namespace Metrics
{
    public interface IMetricsRegistry : IEnumerable<IMetricsHost>
    {
        IEnumerable<KeyValuePair<string, IMetricsHost>> Manifest { get; }

        void Add(IMetricsHost host);
    }
}