using System;
using System.Collections.Generic;
using System.Text;

namespace Cacti.Utils.ObservableUtil
{
    public class Observer<T> : IObserver<T>
    {
        private readonly Action<T> onNext;
        private readonly Action<Exception> onError;
        private readonly Action onCompleted;

        public static Action<Exception> NullOnException = (exception) => { };
        public static Action NullOnCompleted = () => { };

        public Observer(Action<T> onNext)
            : this(onNext, NullOnException, NullOnCompleted)
        {
            
        }

        public Observer(Action<T> onNext, Action<Exception> onException, Action onCompleted)
        {
            this.onNext = onNext ?? throw new ArgumentNullException(nameof(onNext));
            this.onError = onException ?? throw new ArgumentNullException(nameof(onException));
            this.onCompleted = onCompleted ?? throw new ArgumentNullException(nameof(onCompleted));
        }

        public void OnCompleted()
            => onCompleted();

        public void OnError(Exception error)
            => onError(error);

        public void OnNext(T value)
            => onNext(value);
    }
}
