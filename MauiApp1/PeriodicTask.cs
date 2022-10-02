using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class PeriodicTask
    {
        public CancellationTokenSource CancelationTokenSource { get; set; }

        private async Task RunAsync(Action action, TimeSpan period, CancellationToken? cancellationToken)
        {
            while (!(cancellationToken?.IsCancellationRequested ?? false))
            {
                if (cancellationToken == null)
                {
                    await Task.Delay(period);
                }
                else
                {
                    await Task.Delay(period, cancellationToken.Value);
                }

                if (!(cancellationToken?.IsCancellationRequested ?? false))
                    action();
            }
        }

        public Task RunAsync(Action action, TimeSpan period)
        {
            return RunAsync(action, period, CancelationTokenSource?.Token);
        }
    }
}
