using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Metrics
{
    /// <summary>
    ///     A timer metric which aggregates timing durations and provides duration statistics, plus throughput statistics via
    ///     <see cref="MeterMetric" />.
    /// </summary>
    public class TimerMetric : IMetric, IMetered, IDistributed, IComparable<TimerMetric>, IComparable
    {
        private readonly HistogramMetric _histogram;
        private readonly MeterMetric _meter;

        internal TimerMetric(MetricName metricName, TimeUnit durationUnit) : this(durationUnit, TimeUnit.Seconds,
            MeterMetric.New(metricName, "updates", TimeUnit.Seconds),
            new HistogramMetric(metricName, SampleType.Biased), true)
        {
            Name = metricName;
        }

        internal TimerMetric(MetricName metricName, TimeUnit durationUnit, TimeUnit rateUnit) : this(durationUnit,
            rateUnit,
            MeterMetric.New(metricName, "updates", rateUnit), new HistogramMetric(metricName, SampleType.Biased),
            true)
        {
            Name = metricName;
        }

        private TimerMetric(TimeUnit durationUnit, TimeUnit rateUnit, MeterMetric meter, HistogramMetric histogram,
            bool clear)
        {
            DurationUnit = durationUnit;
            RateUnit = rateUnit;
            _meter = meter;
            _histogram = histogram;
            if (clear) Clear();
        }

        /// <summary>
        ///     Returns the timer's duration scale unit
        /// </summary>
        public TimeUnit DurationUnit { get; }

        /// <summary>
        ///     Returns a list of all recorded durations in the timer's sample
        /// </summary>
        public ICollection<double> Values => _histogram.Values.Select(value => ConvertFromNanoseconds(value)).ToList();

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is TimerMetric other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(TimerMetric)}");
        }

        public int CompareTo(TimerMetric other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var durationUnitComparison = DurationUnit.CompareTo(other.DurationUnit);
            if (durationUnitComparison != 0) return durationUnitComparison;
            return RateUnit.CompareTo(other.RateUnit);
        }

        /// <summary>
        ///     Returns the longest recorded duration
        /// </summary>
        public double Max => ConvertFromNanoseconds(_histogram.Max);

        /// <summary>
        ///     Returns the shortest recorded duration
        /// </summary>
        public double Min => ConvertFromNanoseconds(_histogram.Min);

        /// <summary>
        ///     Returns the arithmetic mean of all recorded durations
        /// </summary>
        public double Mean => ConvertFromNanoseconds(_histogram.Mean);

        /// <summary>
        ///     Returns the standard deviation of all recorded durations
        /// </summary>
        public double StdDev => ConvertFromNanoseconds(_histogram.StdDev);

        /// <summary>
        ///     Returns an array of durations at the given percentiles
        /// </summary>
        public double[] Percentiles(params double[] percentiles)
        {
            var scores = _histogram.Percentiles(percentiles);
            for (var i = 0; i < scores.Length; i++) scores[i] = ConvertFromNanoseconds(scores[i]);

            return scores;
        }

        /// <summary>
        ///     Returns the meter's rate unit
        /// </summary>
        /// <returns></returns>
        public TimeUnit RateUnit { get; }

        /// <summary>
        ///     Returns the number of events which have been marked
        /// </summary>
        /// <returns></returns>
        public long Count => _histogram.Count;

        /// <summary>
        ///     Returns the fifteen-minute exponentially-weighted moving average rate at
        ///     which events have occured since the meter was created
        ///     <remarks>
        ///         This rate has the same exponential decay factor as the fifteen-minute load
        ///         average in the top Unix command.
        ///     </remarks>
        /// </summary>
        public double FifteenMinuteRate => _meter.FifteenMinuteRate;

        /// <summary>
        ///     Returns the five-minute exponentially-weighted moving average rate at
        ///     which events have occured since the meter was created
        ///     <remarks>
        ///         This rate has the same exponential decay factor as the five-minute load
        ///         average in the top Unix command.
        ///     </remarks>
        /// </summary>
        public double FiveMinuteRate => _meter.FiveMinuteRate;

        /// <summary>
        ///     Returns the mean rate at which events have occured since the meter was created
        /// </summary>
        public double MeanRate => _meter.MeanRate;

        /// <summary>
        ///     Returns the one-minute exponentially-weighted moving average rate at
        ///     which events have occured since the meter was created
        ///     <remarks>
        ///         This rate has the same exponential decay factor as the one-minute load
        ///         average in the top Unix command.
        ///     </remarks>
        /// </summary>
        /// <returns></returns>
        public double OneMinuteRate => _meter.OneMinuteRate;

        /// <summary>
        ///     Returns the type of events the meter is measuring
        /// </summary>
        /// <returns></returns>
        public string EventType => _meter.EventType;

        [IgnoreDataMember] public MetricName Name { get; }

        public int CompareTo(IMetric other)
        {
            return other.Name.CompareTo(Name);
        }

        internal IMetric Copy()
        {
            var copy = new TimerMetric(DurationUnit, RateUnit, _meter, _histogram, false);
            return copy;
        }

        /// <summary>
        ///     Clears all recorded durations
        /// </summary>
        public void Clear()
        {
            _histogram.Clear();
        }

        public void Update(long duration, TimeUnit durationUnit)
        {
            Update(durationUnit.ToNanos(duration));
        }

        private void Update(long duration)
        {
            if (duration < 0) return;

            _histogram.Update(duration);
            _meter.Mark();
        }

        /// <summary>
        ///     Times and records the duration of an event
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the event</typeparam>
        /// <param name="event">A function whose duration should be timed</param>
        public TimerHandle<T> Time<T>(Func<T> @event)
        {
            var handle = new TimerHandle<T>(new Stopwatch());
            try
            {
                handle.Start();
                handle.Value = @event();
                return handle;
            }
            finally
            {
                Update(handle.Elapsed.Ticks);
                handle.Stop();
            }
        }

        private double ConvertFromNanoseconds(double value)
        {
            return value / DurationUnit.Convert(1, TimeUnit.Nanoseconds);
        }

        public static bool operator <(TimerMetric left, TimerMetric right)
        {
            return Comparer<TimerMetric>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(TimerMetric left, TimerMetric right)
        {
            return Comparer<TimerMetric>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(TimerMetric left, TimerMetric right)
        {
            return Comparer<TimerMetric>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(TimerMetric left, TimerMetric right)
        {
            return Comparer<TimerMetric>.Default.Compare(left, right) >= 0;
        }
    }
}