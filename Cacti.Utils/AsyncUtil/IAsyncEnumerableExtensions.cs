using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.AsyncUtil
{
    public static class IAsyncEnumerableExtensions
    {
        public static IAsyncEnumerable<T> Catch<T, TException>(this IAsyncEnumerable<T> asyncEnumerable, Action<TException> handler)
            where TException : Exception
        {
            return new CompositeAsyncEnumerable<T, T>(asyncEnumerable, value => value, async (token, next) =>
            {
                Func<Task<bool>> moveNext = null;

                moveNext = async () =>
                {
                    bool hasNext = false;
                    try
                    {
                        hasNext = await next();
                    }
                    catch (TException exception)
                    {
                        handler(exception);
                        return await moveNext();
                    }
                    return hasNext;
                };

                return await moveNext();
            });
        }

        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable, CancellationToken token)
        {
            List<T> list = new List<T>();
            await asyncEnumerable.ForEach((value) => list.Add(value), token);
            return list;
        }

        public static IAsyncEnumerable<T> Delay<T>(this IAsyncEnumerable<T> asyncEnumerable, TimeSpan minDelay)
                => new CompositeAsyncEnumerable<T, T>(asyncEnumerable, value => value, async (token, next) =>
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    bool hasNext = await next();
                    stopwatch.Stop();

                    if (hasNext)
                    {
                        TimeSpan delay = minDelay - stopwatch.Elapsed;
                        delay = delay < TimeSpan.Zero
                        ? TimeSpan.Zero
                        : delay;

                        await Task.Delay(delay, token);
                    }
                    return hasNext;
                });

        public static IAsyncEnumerable<T> Where<T>(this IAsyncEnumerable<T> asyncEnumerable, Predicate<T> predicate)
            => new FilterAsyncEnumerable<T>(asyncEnumerable, predicate);

        public static IAsyncEnumerable<TOut> Select<TIn, TOut>(this IAsyncEnumerable<TIn> asyncEnumerable, Func<TIn, TOut> func)
            => new CompositeAsyncEnumerable<TIn, TOut>(asyncEnumerable, func);

        public static Task ForEach<T>(this IAsyncEnumerable<T> asyncEnumerable, Action<T> func, CancellationToken token)
            => ForEach(asyncEnumerable, (value, _token) =>
            {
                func(value);
                return Task.CompletedTask;
            }, token);

        public static async Task ForEach<T>(this IAsyncEnumerable<T> asyncEnumerable, Func<T, CancellationToken, Task> func, CancellationToken token)
        {
            if (asyncEnumerable == null) throw new ArgumentNullException(nameof(asyncEnumerable));
            if (func == null) throw new ArgumentNullException(nameof(func));

            using (IAsyncEnumerator<T> asyncEnumerator = asyncEnumerable.GetAsyncEnumerator())
            {
                while (await asyncEnumerator.MoveNextAsync(token))
                {
                    await func(asyncEnumerator.Current, token);
                }
            }
        }
    }
}
