namespace Metrics
{
    public class MetricsOptions
    {
        public MetricType TypeFilter { get; set; } = MetricType.None;
        public int SampleTimeoutSeconds { get; set; } = 5;
        public bool EnableServerTiming { get; set; } = true;
    }
}