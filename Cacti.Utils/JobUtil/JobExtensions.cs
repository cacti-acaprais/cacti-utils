using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.JobUtil
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

        public static IJob Times(this IJob job, int times, Func<bool> until)
            => Repeat(job, TimeSpan.Zero, () => times-- > 0 && until());

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

        public static IJob Handle<T>(this IJob job)
            where T : Exception
            => Handle<T>(job, (exception) => { });

        public static IJob Handle<T>(this IJob job, Action<T> handle)
            where T : Exception
        {
            return new Job(async (token) =>
            {
                try
                {
                    await job.Execute(token);
                }
                catch (T exception)
                {
                    handle(exception);
                }
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
    }
}
