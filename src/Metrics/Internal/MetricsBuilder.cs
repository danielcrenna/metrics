using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Metrics.Internal
{
    internal class MetricsBuilder : IMetricsBuilder
    {
        private readonly IHealthChecksBuilder _builder;
        private readonly Lazy<MetricsHost> _host;

        public MetricsBuilder(IServiceCollection services, Lazy<MetricsHost> host, IHealthChecksBuilder builder)
        {
            _host = host;
            _builder = builder;
            Services = services;
        }

        public IHealthChecksBuilder Add(HealthCheckRegistration registration)
        {
            return _builder.Add(registration);
        }

        public IServiceCollection Services { get; }

        public IMetricsBuilder RegisterAsHealthCheck(Func<IMetricsHost, GaugeMetric<bool>> builderFunc,
            HealthStatus onCheckFailure = HealthStatus.Unhealthy)
        {
            return RegisterAsHealthCheck(builderFunc, m => m, onCheckFailure);
        }

        public IMetricsBuilder RegisterAsHealthCheck<T>(Func<IMetricsHost, GaugeMetric<T>> builderFunc,
            Func<T, bool> checkFunc, HealthStatus onCheckFailure = HealthStatus.Unhealthy)
        {
            return RegisterAsHealthCheck(builderFunc, m => m.Value, checkFunc, onCheckFailure);
        }

        public IMetricsBuilder RegisterAsHealthCheck(Func<IMetricsHost, CounterMetric> builderFunc,
            Func<long, bool> checkFunc, HealthStatus onCheckFailure = HealthStatus.Unhealthy)
        {
            return RegisterAsHealthCheck(builderFunc, m => m.Count, checkFunc, onCheckFailure);
        }

        public IMetricsBuilder RegisterAsHealthCheck<TMetric, TValue>(Func<IMetricsHost, TMetric> builderFunc,
            Func<TMetric, TValue> valueFunc, Func<TValue, bool> checkFunc,
            HealthStatus onCheckFailure = HealthStatus.Unhealthy) where TMetric : IMetric
        {
            var name = builderFunc(_host.Value).Name.Name;
            _builder.AddCheck($"health_check.{name}", () =>
            {
                TMetric metric;
                try
                {
                    metric = builderFunc(_host.Value);
                }
                catch (Exception e)
                {
                    return new HealthCheckResult(onCheckFailure, "Could not create metric for health check", e);
                }

                try
                {
                    var value = valueFunc(metric);
                    var result = new HealthCheckResult(checkFunc(value) ? HealthStatus.Healthy : onCheckFailure,
                        $"{name} returned value of {value}");
                    return result;
                }
                catch (Exception e)
                {
                    return new HealthCheckResult(onCheckFailure, e.Message, e);
                }
            });
            return this;
        }
    }
}