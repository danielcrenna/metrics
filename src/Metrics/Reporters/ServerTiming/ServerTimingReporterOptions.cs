namespace Metrics.Reporters.ServerTiming
{
    public class ServerTimingReporterOptions
    {
        public bool Enabled { get; set; } = true;
        public string Filter { get; set; } = "*";
        public ServerTimingRendering Rendering { get; set; } = ServerTimingRendering.Verbose;
        public string AllowedOrigins { get; set; } = "*";
    }
}