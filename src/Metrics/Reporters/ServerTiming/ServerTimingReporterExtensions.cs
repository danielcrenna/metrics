using System;
using Metrics.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Metrics.Reporters.ServerTiming
{
    public static class ServerTimingReporterExtensions
    {
        public static MetricsBuilder AddServerTimingReporter(this MetricsBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IMetricsReporter, ServerTimingReporter>());
            return builder;
        }

        public static MetricsBuilder AddServerTimingReporter(this MetricsBuilder builder,
            Action<ServerTimingReporterOptions> configureAction)
        {
            if (configureAction == null) throw new ArgumentNullException(nameof(configureAction));

            builder.AddServerTimingReporter();
            builder.Services.Configure(configureAction);

            return builder;
        }

        public static IApplicationBuilder UseServerTimingReporter(this IApplicationBuilder app)
        {
            ServerTimingReporter.AppBuilder = app;
            return app;
        }
    }
}