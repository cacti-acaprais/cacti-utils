using Cacti.Utils.AsyncUtil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.ObservableUtil
{
    public class AsyncEnumerableObservable<T> : IObservable<T>
    {
        private readonly IAsyncEnumerable<T> enumerable;

        public AsyncEnumerableObservable(IAsyncEnumerable<T> enumerable)
        {
            this.enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            CancellationTokenDisposable tokenDisposable = new CancellationTokenDisposable();

            enumerable
                .ForEach((value) => observer.OnNext(value), tokenDisposable.Token)
                .ContinueWith((task) =>
                {
                    if(task.IsFaulted)
                    {
                        observer.OnError(task.Exception);
                    }
                    observer.OnCompleted();
                }, tokenDisposable.Token);
            
            return tokenDisposable;
        }
    }
}
