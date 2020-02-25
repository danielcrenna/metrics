namespace Metrics
{
    public interface IKeyValueStore<in TKey, TValue>
    {
        TValue this[TKey key] { get; }
        TValue GetOrAdd(TKey key, TValue value);
        bool TryGetValue(TKey key, out TValue value);
        bool Contains(TKey key);
        void AddOrUpdate<T>(TKey key, T value) where T : TValue;
        bool Clear();
    }
}