using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cacti.Utils.ObservableUtil
{
    public class Observable<T> : IObservable<T>
    {
        public Observable()
        { }

        private readonly IdProvider idProvider 
            = new IdProvider();

        private readonly ConcurrentDictionary<long, IObserver<T>> observers
            = new ConcurrentDictionary<long, IObserver<T>>();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            long id = idProvider.Get();
            //TyrAdd is never false, cause id is guarantee to be different each calls.
            observers.TryAdd(id, observer);

            return new Unsubscriber(observers, id);
        }

        public void Next(T value)
            => OnEachObserver(observer => observer.OnNext(value));

        public void Error(Exception exception)
            => OnEachObserver(observer => observer.OnCompleted());

        public void Complete()
            => OnEachObserver(observer => observer.OnCompleted());

        private void OnEachObserver(Action<IObserver<T>> action)
        {
            IObserver<T>[] observers = this.observers.Values.ToArray();
            foreach (IObserver<T> observer in observers)
                action(observer);
        }

        private class Unsubscriber : IDisposable
        {
            private ConcurrentDictionary<long, IObserver<T>> observers;
            private readonly long id;

            public Unsubscriber(ConcurrentDictionary<long, IObserver<T>> observers, long id)
            {
                this.observers = observers ?? throw new ArgumentNullException(nameof(observers));
                this.id = id;
            }

            public void Dispose()
            {
                observers?.TryRemove(id, out IObserver<T> value);
                observers = null;
            }
        }
    }
}
