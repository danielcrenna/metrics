using System;
using ActiveRoutes;
using Metrics.Controllers;
using Metrics.Features;
using Metrics.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Metrics
{
    public static class Add
    {
        public static IServiceCollection AddMetrics(this IServiceCollection services)
        {
            return AddMetrics(services, builder => { });
        }

        public static IServiceCollection AddMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            return AddMetrics(services, configuration.Bind);
        }

        public static IServiceCollection AddMetrics(this IServiceCollection services, IConfiguration configuration,
            Action<MetricsBuilder> configure)
        {
            return AddMetrics(services, builder =>
            {
                var options = new MetricsOptions();
                configuration.Bind(options);
                configure(builder);
            });
        }

        public static IServiceCollection AddMetrics(this IServiceCollection services, Action<MetricsBuilder> configure)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddOptions();

            var store = new Lazy<InMemoryMetricsStore>(() => new InMemoryMetricsStore());
            var host = new Lazy<MetricsHost>(() => new MetricsHost(store.Value));
            var registry = new Lazy<InMemoryMetricsRegistry>(() => new InMemoryMetricsRegistry {host.Value});

            services.TryAdd(ServiceDescriptor.Singleton<IMetricsStore, InMemoryMetricsStore>(r => store.Value));
            services.TryAdd(ServiceDescriptor.Singleton<IMetricsHost, MetricsHost>(r => host.Value));
            services.TryAdd(ServiceDescriptor.Singleton<IMetricsRegistry>(r => registry.Value));
            services.TryAdd(ServiceDescriptor.Singleton(typeof(IMetricsHost<>), typeof(MetricsHost<>)));

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<MetricsOptions>>(
                new DefaultMetricsConfigureOptions()));

            configure(new MetricsBuilder(services, host, services.AddHealthChecks()));
            return services;
        }

        public static IServiceCollection AddMetricsApi(this IServiceCollection services, Action<MetricsOptions> configureAction = null)
        {
            services.AddActiveRouting(builder =>
            {
                builder.AddMetricsApi(configureAction);
            });

            return services;
        }

        private static void AddMetricsApi(this IMvcCoreBuilder mvcBuilder, Action<MetricsOptions> configureAction = null)
        {
            if (configureAction != null)
                mvcBuilder.Services.Configure(configureAction);

            mvcBuilder.AddActiveRoute<MetricsBuilder, MetricsController, MetricsFeature, MetricsOptions>();
        }
    }
}