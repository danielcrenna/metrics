namespace Metrics
{
    public static class MetricTypeExtensions
    {
        public static bool HasFlagFast(this MetricType value, MetricType flag)
        {
            return (value & flag) != 0;
        }
    }
}