using System;
using System.Threading;
using System.Threading.Tasks;

namespace Metrics.Reporters
{
    public abstract class PeriodicReporter : IMetricsReporter
    {
        private readonly TimeSpan _interval;
        private CancellationTokenSource _cancel;
        private Task _task;

        protected PeriodicReporter(TimeSpan? interval)
        {
            _interval = interval ?? TimeSpan.FromSeconds(5);
        }

        public Task InitializeAsync()
        {
            _cancel = new CancellationTokenSource();
            _task = OnTimer(() => Report(_cancel.Token), _cancel.Token);
            return _task;
        }

        public void Dispose()
        {
            Stop();
        }

        public abstract Task Report(CancellationToken cancellationToken = default);

        public async Task OnTimer(Action action, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_interval, cancellationToken);

                if (!cancellationToken.IsCancellationRequested) action();
            }
        }

        public void Stop()
        {
            if (_task.IsCompleted)
            {
                _task?.Dispose();
            }
            else
            {
                _cancel.Cancel();
                _task?.Dispose();
                _cancel.Dispose();
            }
        }
    }
}