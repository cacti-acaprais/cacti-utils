using Cacti.Utils.JobUtil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.ObservableUtil
{
    public class FuncObservable<T> : IObservable<T>
    {
        private readonly Func<CancellationToken, Task<T>> onExecute;

        public FuncObservable(Func<CancellationToken, Task<T>> onExecute)
        {
            this.onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            onExecute(tokenSource.Token)
                .ContinueWith((task) => 
                {
                    if(task.IsFaulted)
                    {
                        observer.OnError(task.Exception);
                    }
                    observer.OnCompleted();
                }, tokenSource.Token);

            return new Unsubscribe(tokenSource);
        }

        private class Unsubscribe : IDisposable
        {
            private readonly CancellationTokenSource tokenSource;

            public Unsubscribe(CancellationTokenSource tokenSource)
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
