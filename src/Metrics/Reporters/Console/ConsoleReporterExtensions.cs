using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Metrics.Reporters.Console
{
    public static class ConsoleReporterExtensions
    {
        public static IMetricsBuilder AddConsoleReporter(this IMetricsBuilder builder)
        {
            var descriptor = ServiceDescriptor.Singleton<IMetricsReporter, ConsoleReporter>();
            builder.Services.TryAddEnumerable(descriptor);
            return builder;
        }

        public static IMetricsBuilder AddConsoleReporter(this IMetricsBuilder builder,
            Action<ConsoleReporterOptions> configureAction)
        {
            if (configureAction == null) throw new ArgumentNullException(nameof(configureAction));

            builder.AddConsoleReporter();
            builder.Services.Configure(configureAction);

            return builder;
        }
    }
}