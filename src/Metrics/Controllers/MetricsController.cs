// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ActiveRoutes;
using Metrics.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Metrics.Controllers
{
    public class MetricsController : Controller
    {
        private readonly IMetricsRegistry _registry;
        private readonly IOptionsSnapshot<MetricsOptions> _options;

        public MetricsController(IMetricsRegistry registry, IOptionsSnapshot<MetricsOptions> options)
        {
            _registry = registry;
            _options = options;
        }

        [DynamicHttpGet("")]
        public async Task<IActionResult> GetAsync()
        {
            var timeout = TimeSpan.FromSeconds(_options.Value.SampleTimeoutSeconds);
            var cancel = new CancellationTokenSource(timeout);
            var samples = await Task.Run(() => _registry.SelectMany(x => x.GetSample()).ToImmutableDictionary(), cancel.Token);
            var json = JsonSampleSerializer.Serialize(samples);

            return Ok(json);
        }
    }
}