using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TimeCheck.Helpers
{
    public class Debouncer
    {
        private List<CancellationTokenSource> cancellationTokens = new List<CancellationTokenSource>();
        private TimeSpan delay;

        public Debouncer(TimeSpan delay)
        {
            this.delay = delay;
        }

        public void AddAction(Action action, TimeSpan? differentDelay = null)
        {
            var ts = new CancellationTokenSource();

            if (cancellationTokens.Any())
            {
                cancellationTokens.ForEach(x =>
                {
                    if (!x.IsCancellationRequested)
                    {
                        x.Cancel();
                    }
                });
            }

            cancellationTokens = new List<CancellationTokenSource>();

            CancellationToken ct = ts.Token;
            cancellationTokens.Add(ts);


            var taskToRun = new Task(() =>
            {
                if (ct.IsCancellationRequested)
                {
                    // Clean up here, then...
                    ct.ThrowIfCancellationRequested();
                }

                action();
            }, ct);

            var usedDelay = delay;
            if (differentDelay.HasValue)
            {
                usedDelay = differentDelay.Value;
            }

            Task.Delay(usedDelay).Wait();
            taskToRun.RunSynchronously();

            if (taskToRun.IsCompleted)
            {
                cancellationTokens = new List<CancellationTokenSource>();
            }
        }
    }
}
