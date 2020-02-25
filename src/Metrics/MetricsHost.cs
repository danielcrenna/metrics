using System;
using System.Collections.Immutable;

namespace Metrics
{
    public class MetricsHost : IMetricsHost
    {
        private static readonly Type Owner = typeof(MetricsHost);
        private readonly IMetricsStore _store;

        public MetricsHost(IMetricsStore store)
        {
            _store = store;
        }

        public MetricsHost() : this(new InMemoryMetricsStore())
        {
        }

        private T GetOrAdd<T>(MetricName name, T metric) where T : IMetric
        {
            if (_store.Contains(name)) return (T) _store[name];

            _store.AddOrUpdate(name, metric);
            return metric;
        }

        #region Default Owner

        public GaugeMetric<T> Gauge<T>(string name, Func<T> evaluator)
        {
            return Gauge(Owner, name, evaluator);
        }

        public CounterMetric Counter(string name)
        {
            return Counter(Owner, name);
        }

        public HistogramMetric Histogram(string name, SampleType sampleType)
        {
            return Histogram(Owner, name, sampleType);
        }

        public MeterMetric Meter(string name, string eventType, TimeUnit rateUnit)
        {
            return Meter(Owner, name, eventType, rateUnit);
        }

        public TimerMetric Timer(string name, TimeUnit durationUnit, TimeUnit rateUnit)
        {
            return Timer(Owner, name, durationUnit, rateUnit);
        }

        #endregion

        #region Owner

        /// <summary>
        ///     Creates a new gauge metric and registers it under the given type and name
        /// </summary>
        /// <typeparam name="T">The type the gauge measures</typeparam>
        /// <param name="owner">The type that owns the metric</param>
        /// <param name="name">The metric name</param>
        /// <param name="evaluator">The gauge evaluation function</param>
        /// <returns></returns>
        public GaugeMetric<T> Gauge<T>(Type owner, string name, Func<T> evaluator)
        {
            var metricName = new MetricName(owner, name);
            return GetOrAdd(metricName, new GaugeMetric<T>(metricName, evaluator));
        }

        /// <summary>
        ///     Creates a new counter metric and registers it under the given type and name
        /// </summary>
        /// <param name="owner">The type that owns the metric</param>
        /// <param name="name">The metric name</param>
        /// <returns></returns>
        public CounterMetric Counter(Type owner, string name)
        {
            var metricName = new MetricName(owner, name);
            return GetOrAdd(metricName, new CounterMetric(metricName));
        }

        /// <summary>
        ///     Creates a new histogram metric and registers it under the given type and name
        /// </summary>
        /// <param name="owner">The type that owns the metric</param>
        /// <param name="name">The metric name</param>
        /// <param name="sampleType">The sample type</param>
        /// <returns></returns>
        public HistogramMetric Histogram(Type owner, string name, SampleType sampleType)
        {
            var metricName = new MetricName(owner, name);
            return GetOrAdd(metricName, new HistogramMetric(metricName, sampleType));
        }

        /// <summary>
        ///     Creates a new meter metric and registers it under the given type and name
        /// </summary>
        /// <param name="owner">The type that owns the metric</param>
        /// <param name="name">The metric name</param>
        /// <param name="eventType">The plural name of the type of events the meter is measuring (e.g., <code>"requests"</code>)</param>
        /// <param name="rateUnit">The rate unit of the new meter</param>
        /// <returns></returns>
        public MeterMetric Meter(Type owner, string name, string eventType, TimeUnit rateUnit)
        {
            var metricName = new MetricName(owner, name);
            if (_store.TryGetValue(metricName, out var existingMetric)) return (MeterMetric) existingMetric;

            var metric = MeterMetric.New(metricName, eventType, rateUnit);
            var added = _store.GetOrAdd(metricName, metric);
            return added == null ? metric : (MeterMetric) added;
        }

        /// <summary>
        ///     Creates a new timer metric and registers it under the given type and name
        /// </summary>
        /// <param name="owner">The type that owns the metric</param>
        /// <param name="name">The metric name</param>
        /// <param name="durationUnit">The duration scale unit of the new timer</param>
        /// <param name="rateUnit">The rate unit of the new timer</param>
        /// <returns></returns>
        public TimerMetric Timer(Type owner, string name, TimeUnit durationUnit, TimeUnit rateUnit)
        {
            var metricName = new MetricName(owner, name);
            if (_store.TryGetValue(metricName, out var existingMetric)) return (TimerMetric) existingMetric;

            var metric = new TimerMetric(metricName, durationUnit, rateUnit);
            var justAddedMetric = _store.GetOrAdd(metricName, metric);
            return justAddedMetric == null ? metric : (TimerMetric) justAddedMetric;
        }

        /// <summary>
        ///     Returns a copy of all currently registered metrics in an immutable collection
        /// </summary>
        public IImmutableDictionary<MetricName, IMetric> GetSample(MetricType typeFilter = MetricType.None)
        {
            return _store.GetSample(typeFilter);
        }

        /// <summary>
        ///     Clears all previously registered metrics, if the underlying <see cref="IMetricsStore" /> supports
        ///     <see cref="IClearable" />
        /// </summary>
        public bool Clear()
        {
            return _store.Clear();
        }

        #endregion
    }
}