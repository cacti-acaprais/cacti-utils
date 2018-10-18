using Cacti.Utils.AsyncUtil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.ObservableUtil
{
    public static class IObservableExtensions
    {
        public static IObservable<T> ToObservable<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));

            EnumerableObservable<T> observable = new EnumerableObservable<T>(enumerable);
            return observable;
        }

        public static IObservable<T> ToObservable<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            if (asyncEnumerable == null) throw new ArgumentNullException(nameof(asyncEnumerable));

            AsyncEnumerableObservable<T> observable = new AsyncEnumerableObservable<T>(asyncEnumerable);
            return observable;
        }
    }
}
