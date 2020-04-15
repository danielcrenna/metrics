using System;
using System.Collections.Generic;
using ActiveRoutes;
using Metrics.Controllers;

namespace Metrics.Features
{
    internal class MetricsFeature : DynamicFeature
    {
        public override IList<Type> ControllerTypes { get; }
        public MetricsFeature() => ControllerTypes = new List<Type> {typeof(MetricsController)};
    }
}
