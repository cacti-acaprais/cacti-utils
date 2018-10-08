using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils
{
    public static class JobExtensions
    {
        public static IJob Delay(this IJob job, TimeSpan delay)
        {
            return new Job(async (token) =>
            {
                await Task.Delay(delay, token);
                await job.Execute(token);
            });
        }

        public static IJob Times(this IJob job, int times)
            => Times(job, times, TimeSpan.Zero);

        public static IJob Times(this IJob job, int times, TimeSpan delay)
            => Repeat(job, delay, () => times-- > 0);

        public static IJob Repeat(this IJob job, TimeSpan delay)
            => Repeat(job, delay, () => true);

        public static IJob Repeat(this IJob job, TimeSpan delay, Func<bool> until)
        {
            return new Job(async (token) =>
            {
                while (!token.IsCancellationRequested && until())
                {
                    await job.Execute(token);
                    await Task.Delay(delay, token);
                }
            });
        }

        public static IJob Then(this IJob job, IJob nextJob)
        {
            return new Job(async (token) =>
            {
                await job.Execute(token);

                if (!token.IsCancellationRequested)
                    await nextJob.Execute(token);
            });
        }

        public static IJob Aggregate(this IEnumerable<IJob> jobs)
        {
            IJob[] _jobs = jobs.ToArray();

            return new Job(async (token) =>
            {
                Task[] tasks = _jobs
                    .Select(job => job.Execute(token))
                    .ToArray();

                await Task.WhenAll(tasks);
            });
        }

        public static IDisposable Run(this IJob job)
            => Run(job, (exception) => { });

        public static IDisposable Run(this IJob job, Action<Exception> handler)
            => Run(job, exception =>
            {
                handler(exception);
                return false;
            });

        public static IDisposable Run(this IJob job, Func<Exception, bool> handler)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            var cancelTokenSource = new CancellationTokenSource();

            Task task = Run(job, handler, cancelTokenSource);

            return new CancelJob(task, cancelTokenSource);
        }

        private static Task Run(IJob job, Func<Exception, bool> handler, CancellationTokenSource tokenSource)
        {
            Task task = Task.Run(async () =>
            {
                try
                {
                    await Task.Run(async () => await job.Execute(tokenSource.Token), tokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception exception)
                {
                    if (handler(exception))
                    {
                        await Run(job, handler, tokenSource);
                    }
                }
            });

            return task;
        }

        private class CancelJob : IDisposable
        {
            private readonly Task task;
            private readonly CancellationTokenSource tokenSource;

            public CancelJob(Task task, CancellationTokenSource cancelTokenSource)
            {
                this.task = task ?? throw new ArgumentNullException(nameof(task));
                this.tokenSource = cancelTokenSource ?? throw new ArgumentNullException(nameof(cancelTokenSource));
            }

            public void Dispose()
            {
                tokenSource.Cancel();
                task.Wait();
                tokenSource.Dispose();
            }
        }
    }
}
