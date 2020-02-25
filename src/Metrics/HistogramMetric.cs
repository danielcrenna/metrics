using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Metrics.Internal;
using Metrics.Stats;

namespace Metrics
{
    /// <summary>
    ///     A metric which calculates the distribution of a value
    ///     <see href="http://www.johndcook.com/standard_deviation.html">Accurately computing running variance</see>
    /// </summary>
    public class HistogramMetric : IMetric, IDistributed, IComparable<HistogramMetric>, IComparable
    {
        private readonly ISample _sample;

        private readonly AtomicLong _count = new AtomicLong();
        private readonly AtomicLong _max = new AtomicLong();
        private readonly AtomicLong _min = new AtomicLong();
        private readonly AtomicLong _sum = new AtomicLong();
        
        // These are for the Welford algorithm for calculating running variance without floating-point doom
        private readonly AtomicLong _varianceM = new AtomicLong();
        private readonly AtomicLong _varianceS = new AtomicLong();

        /// <summary>
        ///     Creates a new <see cref="T:Metrics.HistogramMetric" /> with the given sample type
        /// </summary>
        internal HistogramMetric(MetricName metricName, SampleType type) : this(metricName, NewSample(type))
        {
        }

        /// <summary>
        ///     Creates a new <see cref="HistogramMetric" /> with the given sample
        /// </summary>
        internal HistogramMetric(MetricName metricName, ISample sample) : this(metricName, sample, true)
        {
            _sample = sample;
            Clear();
        }

        private HistogramMetric(MetricName metricName, ISample sample, bool clear)
        {
            Name = metricName;
            _sample = sample;
            if (clear) Clear();
        }

        /// <summary>
        ///     Returns a list of all values in the histogram's sample
        /// </summary>
        public ICollection<long> Values => _sample.Values;

        private double Variance => Count <= 1 ? 0.0 : BitConverter.Int64BitsToDouble(_varianceS.Get()) / (Count - 1);

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is HistogramMetric other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(HistogramMetric)}");
        }

        public int CompareTo(HistogramMetric other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Name.CompareTo(other.Name);
        }

        /// <summary>
        ///     Returns the number of values recorded
        /// </summary>
        public long Count => _count.Get();

        /// <summary>
        ///     Returns the largest recorded value
        /// </summary>
        public double Max => Count > 0 ? _max.Get() : 0.0;

        /// <summary>
        ///     Returns the smallest recorded value
        /// </summary>
        public double Min => Count > 0 ? _min.Get() : 0.0;

        /// <summary>
        ///     Returns the arithmetic mean of all recorded values
        /// </summary>
        public double Mean => Count > 0 ? _sum.Get() / (double) Count : 0.0;

        /// <summary>
        ///     Returns the standard deviation of all recorded values
        /// </summary>
        public double StdDev => Count > 0 ? Math.Sqrt(Variance) : 0.0;

        /// <summary>
        ///     Returns an array of values at the given percentiles
        /// </summary>
        public double[] Percentiles(params double[] percentiles)
        {
            var scores = new double[percentiles.Length];
            for (var i = 0; i < scores.Length; i++) scores[i] = 0.0;

            if (Count > 0)
            {
                var values = _sample.Values.OrderBy(v => v).ToList();

                for (var i = 0; i < percentiles.Length; i++)
                {
                    var p = percentiles[i];
                    var pos = p * (values.Count + 1);
                    if (pos < 1)
                    {
                        scores[i] = values[0];
                    }
                    else if (pos >= values.Count)
                    {
                        scores[i] = values[values.Count - 1];
                    }
                    else
                    {
                        var lower = values[(int) pos - 1];
                        var upper = values[(int) pos];
                        scores[i] = lower + (pos - Math.Floor(pos)) * (upper - lower);
                    }
                }
            }

            return scores;
        }

        [IgnoreDataMember] public MetricName Name { get; }

        public int CompareTo(IMetric other)
        {
            return other.Name.CompareTo(Name);
        }

        public IMetric Copy()
        {
            var copy = new HistogramMetric(Name, _sample);
            return copy;
        }

        /// <summary>
        ///     Clears all recorded values
        /// </summary>
        public void Clear()
        {
            _sample.Clear();
            _count.Set(0);
            _max.Set(long.MinValue);
            _min.Set(long.MaxValue);
            _sum.Set(0);
            _varianceM.Set(-1);
            _varianceS.Set(0);
        }

        /// <summary>
        ///     Adds a recorded value
        /// </summary>
        public void Update(int value)
        {
            Update((long) value);
        }

        /// <summary>
        ///     Adds a recorded value
        /// </summary>
        public void Update(long value)
        {
            _count.IncrementAndGet();
            _sample.Update(value);
            SetMax(value);
            SetMin(value);
            _sum.GetAndAdd(value);
            UpdateVariance(value);
        }

        private void SetMax(long potentialMax)
        {
            var done = false;
            while (!done)
            {
                var currentMax = _max.Get();
                done = currentMax >= potentialMax || _max.CompareAndSet(currentMax, potentialMax);
            }
        }

        private void SetMin(long potentialMin)
        {
            var done = false;
            while (!done)
            {
                var currentMin = _min.Get();
                done = currentMin <= potentialMin || _min.CompareAndSet(currentMin, potentialMin);
            }
        }

        private void UpdateVariance(long value)
        {
            // Initialize varianceM to the first reading if it's still blank
            if (_varianceM.CompareAndSet(-1, BitConverter.DoubleToInt64Bits(value))) return;

            var done = false;
            while (!done)
            {
                var oldMCas = _varianceM.Get();
                var oldM = BitConverter.Int64BitsToDouble(oldMCas);
                var newM = oldM + (value - oldM) / Count;
                var oldSCas = _varianceS.Get();
                var oldS = BitConverter.Int64BitsToDouble(oldSCas);
                var newS = oldS + (value - oldM) * (value - newM);

                done = _varianceM.CompareAndSet(oldMCas, BitConverter.DoubleToInt64Bits(newM)) &&
                       _varianceS.CompareAndSet(oldSCas, BitConverter.DoubleToInt64Bits(newS));
            }
        }

        private static ISample NewSample(SampleType type)
        {
            switch (type)
            {
                case SampleType.Uniform:
                    return new UniformSample(1028);
                case SampleType.Biased:
                    return new ExponentiallyDecayingSample(1028, 0.015);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public static bool operator <(HistogramMetric left, HistogramMetric right)
        {
            return Comparer<HistogramMetric>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(HistogramMetric left, HistogramMetric right)
        {
            return Comparer<HistogramMetric>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(HistogramMetric left, HistogramMetric right)
        {
            return Comparer<HistogramMetric>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(HistogramMetric left, HistogramMetric right)
        {
            return Comparer<HistogramMetric>.Default.Compare(left, right) >= 0;
        }
    }
}