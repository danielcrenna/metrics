using System;

namespace Metrics
{
    /// <summary>
    ///     A hash key for storing metrics associated by the parent class and name pair
    /// </summary>
    public struct MetricName : IComparable<MetricName>, IComparable

    {
        public Type Class { get; }

        public string Name { get; }

        public MetricName(Type @class, string name) : this()
        {
            Class = @class;
            Name = name;
        }

        public bool Equals(MetricName other)
        {
            return Equals(other.Name, Name) && other.Class == Class;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            return obj is MetricName name && Equals(name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Class != null ? Class.GetHashCode() : 0);
            }
        }

        public static bool operator ==(MetricName left, MetricName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MetricName left, MetricName right)
        {
            return !left.Equals(right);
        }

        public string CacheKey => $"{Class.FullName}.{Name}";

        public int CompareTo(MetricName other)
        {
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is MetricName other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(MetricName)}");
        }

        public static bool operator <(MetricName left, MetricName right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(MetricName left, MetricName right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(MetricName left, MetricName right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(MetricName left, MetricName right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}