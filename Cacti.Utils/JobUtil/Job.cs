using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.JobUtil
{
    public class Job : IJob
    {
        private readonly Func<CancellationToken, Task> action;

        public Job(Action action)
            : this(() =>
            {
                action();
                return Task.CompletedTask;
            })
        {
        }

        public Job(Func<Task> action)
            : this(async (CancellationToken token) => await action())
        {

        }

        public Job(IJob job)
            : this(job.Execute)
        {

        }

        public Job(Func<CancellationToken, Task> action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public async Task Execute(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                throw new TaskCanceledException();

            await action(token);
        }
    }
}
