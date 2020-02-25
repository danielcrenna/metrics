using System;

namespace Metrics
{
    [Flags]
    public enum MetricType
    {
        Gauge = (byte) 1u << 0,
        Counter = (byte) 1u << 1,
        Meter = (byte) 1u << 2,
        Histogram = (byte) 1u << 3,
        Timer = (byte) 1u << 4,

        None = 0,
        All = 0xFF
    }
}