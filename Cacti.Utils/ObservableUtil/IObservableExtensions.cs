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
        public static void Observe<T>(this IEnumerable<T> enumerable, IObserver<T> observer)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            Observable<T> observable = new Observable<T>();
            observable.Subscribe(observer);

            foreach(T value in enumerable)
            {
                observable.Next(value);
            }
            observable.Dispose();
        }

        public static async Task Observe<T>(this IAsyncEnumerable<T> asyncEnumerable, IObserver<T> observer, CancellationToken token)
        {
            if (asyncEnumerable == null) throw new ArgumentNullException(nameof(asyncEnumerable));
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (token == null) throw new ArgumentNullException(nameof(token));

            Observable<T> observable = new Observable<T>();
            observable.Subscribe(observer);

            await asyncEnumerable.ForEach(value => observable.Next(value), token);
            
            observable.Dispose();
        }
    }
}
