using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
