using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Metrics
{
    public interface IMetricsBuilder : IHealthChecksBuilder
    {
        IMetricsBuilder RegisterAsHealthCheck(Func<IMetricsHost, GaugeMetric<bool>> builderFunc,
            HealthStatus onCheckFailure = HealthStatus.Unhealthy);

        IMetricsBuilder RegisterAsHealthCheck<T>(Func<IMetricsHost, GaugeMetric<T>> builderFunc,
            Func<T, bool> checkFunc, HealthStatus onCheckFailure = HealthStatus.Unhealthy);

        IMetricsBuilder RegisterAsHealthCheck(Func<IMetricsHost, CounterMetric> builderFunc, Func<long, bool> checkFunc,
            HealthStatus onCheckFailure = HealthStatus.Unhealthy);

        IMetricsBuilder RegisterAsHealthCheck<TMetric, TValue>(Func<IMetricsHost, TMetric> builderFunc,
            Func<TMetric, TValue> valueFunc, Func<TValue, bool> checkFunc,
            HealthStatus onCheckFailure = HealthStatus.Unhealthy) where TMetric : IMetric;
    }
}