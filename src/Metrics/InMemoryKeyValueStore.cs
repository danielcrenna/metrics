using System.Collections.Concurrent;

namespace Metrics
{
    public class InMemoryKeyValueStore<TKey, TValue> : IKeyValueStore<TKey, TValue>
    {
        protected readonly ConcurrentDictionary<TKey, TValue> Memory;

        public InMemoryKeyValueStore() => Memory = new ConcurrentDictionary<TKey, TValue>();

        public TValue GetOrAdd(TKey name, TValue metric)
        {
            return Memory.GetOrAdd(name, metric);
        }

        public TValue this[TKey name] => Memory[name];

        public bool TryGetValue(TKey name, out TValue value)
        {
            return Memory.TryGetValue(name, out value);
        }

        public bool Contains(TKey name)
        {
            return Memory.ContainsKey(name);
        }

        public void AddOrUpdate<T>(TKey name, T value) where T : TValue
        {
            Memory.AddOrUpdate(name, value, (n, m) => m);
        }

        public bool Clear()
        {
            Memory.Clear();
            return true;
        }
    }
}