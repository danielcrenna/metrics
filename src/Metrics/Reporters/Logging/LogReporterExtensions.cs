using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Metrics.Reporters.Logging
{
    public static class LogReporterExtensions
    {
        public static IMetricsBuilder AddLogReporter(this IMetricsBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IMetricsReporter, LogReporter>());
            return builder;
        }

        public static IMetricsBuilder AddLogReporter(this IMetricsBuilder builder,
            Action<LogReporterOptions> configureAction)
        {
            if (configureAction == null) throw new ArgumentNullException(nameof(configureAction));

            builder.AddLogReporter();
            builder.Services.Configure(configureAction);

            return builder;
        }
    }
}