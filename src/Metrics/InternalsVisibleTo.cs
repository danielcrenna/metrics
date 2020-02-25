using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HQ.Extensions.Metrics.AspNetCore")]
[assembly: InternalsVisibleTo("HQ.Extensions.Metrics.Tests")]
[assembly: InternalsVisibleTo("HQ.Extensions.Metrics.Reporters.Console")]
[assembly: InternalsVisibleTo("HQ.Extensions.Metrics.Reporters.Logging")]
[assembly: InternalsVisibleTo("HQ.Extensions.Metrics.Reporters.ServerTiming")]

namespace Metrics
{
    internal class InternalsVisibleTo
    {
    }
}