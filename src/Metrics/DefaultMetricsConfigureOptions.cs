using System;
using Microsoft.Extensions.Options;

namespace Metrics
{
    internal class DefaultMetricsConfigureOptions : ConfigureOptions<MetricsOptions>
    {
        public DefaultMetricsConfigureOptions() : base(DefaultOptionsBuilder())
        {
        }

        private static Action<MetricsOptions> DefaultOptionsBuilder()
        {
            return options =>
            {
                options.SampleTimeoutSeconds = 5;
                options.TypeFilter = MetricType.None;
            };
        }
    }
}