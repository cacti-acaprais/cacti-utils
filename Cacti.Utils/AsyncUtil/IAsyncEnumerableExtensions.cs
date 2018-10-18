using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.AsyncUtil
{
    public static class IAsyncEnumerableExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable, CancellationToken token)
        {
            List<T> list = new List<T>();
            await asyncEnumerable.ForEach((value) => list.Add(value), token);
            return list;
        }

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

            IAsyncEnumerator<T> asyncEnumerator = asyncEnumerable.GetAsyncEnumerator();

            while (await asyncEnumerator.MoveNextAsync(token))
            {
                await func(asyncEnumerator.Current, token);
            }
        }
    }
}
