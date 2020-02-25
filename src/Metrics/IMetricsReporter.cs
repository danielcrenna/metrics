using System;
using System.Threading.Tasks;

namespace Metrics
{
    public interface IMetricsReporter : IDisposable
    {
        Task InitializeAsync();
    }
}