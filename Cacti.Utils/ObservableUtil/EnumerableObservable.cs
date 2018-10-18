using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Cacti.Utils.ObservableUtil
{
    public class EnumerableObservable<T> : IObservable<T>
    {
        private readonly IEnumerable<T> enumerable;

        public EnumerableObservable(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            try
            {
                foreach (T value in enumerable)
                {
                    if (token.IsCancellationRequested)
                        break;

                    observer.OnNext(value);
                }
            }
            catch (Exception exception)
            {
                observer.OnError(exception);
            }

            observer.OnCompleted();

            return new Unsubscriber(tokenSource);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly CancellationTokenSource tokenSource;

            public Unsubscriber(CancellationTokenSource tokenSource)
            {
                this.tokenSource = tokenSource ?? throw new ArgumentNullException(nameof(tokenSource));
            }

            public void Dispose()
            {
                tokenSource.Cancel();
            }
        }
    }
}
