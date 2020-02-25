using System;
using System.Collections.Immutable;

namespace Metrics
{
    public class MetricsHost<TOwner> : IMetricsHost<TOwner>
    {
        private readonly IMetricsHost _host;

        public MetricsHost(IMetricsHost host)
        {
            _host = host;
        }

        public MetricsHost() : this(new MetricsHost(new InMemoryMetricsStore()))
        {
        }

        /// <summary>
        ///     Creates a new gauge metric and registers it under the given type and name
        /// </summary>
        /// <typeparam name="T">The type the gauge measures</typeparam>
        /// <param name="name">The metric name</param>
        /// <param name="evaluator">The gauge evaluation function</param>
        /// <returns></returns>
        public GaugeMetric<T> Gauge<T>(string name, Func<T> evaluator)
        {
            return _host.Gauge(typeof(TOwner), name, evaluator);
        }

        /// <summary>
        ///     Creates a new counter metric and registers it under the given type and name
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <returns></returns>
        public CounterMetric Counter(string name)
        {
            return _host.Counter(typeof(TOwner), name);
        }

        /// <summary>
        ///     Creates a new histogram metric and registers it under the given type and name
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="sampleType">The sample type</param>
        /// <returns></returns>
        public HistogramMetric Histogram(string name, SampleType sampleType)
        {
            return _host.Histogram(typeof(TOwner), name, sampleType);
        }

        /// <summary>
        ///     Creates a new meter metric and registers it under the given type and name
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="eventType">The plural name of the type of events the meter is measuring (e.g., <code>"requests"</code>)</param>
        /// <param name="rateUnit">The rate unit of the new meter</param>
        /// <returns></returns>
        public MeterMetric Meter(string name, string eventType, TimeUnit rateUnit)
        {
            return _host.Meter(typeof(TOwner), name, eventType, rateUnit);
        }

        /// <summary>
        ///     Creates a new timer metric and registers it under the given type and name
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="durationUnit">The duration scale unit of the new timer</param>
        /// <param name="rateUnit">The rate unit of the new timer</param>
        /// <returns></returns>
        public TimerMetric Timer(string name, TimeUnit durationUnit, TimeUnit rateUnit)
        {
            return _host.Timer(typeof(TOwner), name, durationUnit, rateUnit);
        }

        /// <summary>
        ///     Clears all previously registered metrics
        /// </summary>
        public void Clear()
        {
            _host.Clear();
        }

        public IImmutableDictionary<MetricName, IMetric> GetSample(MetricType typeFilter = MetricType.None)
        {
            return _host.GetSample(typeFilter);
        }
    }
}