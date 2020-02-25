using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Metrics
{
    public class InMemoryMetricsRegistry : IMetricsRegistry
    {
        private readonly ConcurrentDictionary<string, IMetricsHost> _registry;

        public InMemoryMetricsRegistry()
        {
            _registry = new ConcurrentDictionary<string, IMetricsHost>();
        }

        public IEnumerable<KeyValuePair<string, IMetricsHost>> Manifest => _registry;

        public void Add(IMetricsHost host)
        {
            var key = $"{Environment.MachineName}.{Environment.CurrentManagedThreadId}";
            _registry.AddOrUpdate(key, host, (n, r) => r);
        }

        public IEnumerator<IMetricsHost> GetEnumerator()
        {
            return _registry.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}