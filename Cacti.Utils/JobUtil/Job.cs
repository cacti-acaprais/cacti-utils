using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.JobUtil
{
    public class Job : IJob
    {
        private static Action NullDispose => () => {};

        public Job(Action action)
            : this(action, NullDispose)
        { }

        public Job(Action action, Action onDispose)
            : this(() =>
            {
                action();
                return Task.CompletedTask;
            }, onDispose)
        {
        }

        public Job(Func<Task> action)
            : this(action, NullDispose)
        { }

        public Job(Func<Task> action, Action onDispose)
            : this(async (CancellationToken token) => await action(), onDispose)
        {

        }

        public Job(IJob job)
            : this(job.Execute, job.Dispose)
        {

        }

        public Job(Func<CancellationToken, Task> action)
            : this(action, NullDispose)
        { }

        public Job(Func<CancellationToken, Task> action, Action onDispose)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        private readonly Func<CancellationToken, Task> action;
        private readonly Action onDispose;

        public async Task Execute(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                throw new TaskCanceledException();

            await action(token);
        }

        public void Dispose()
        {
            onDispose();
        }
    }
}
