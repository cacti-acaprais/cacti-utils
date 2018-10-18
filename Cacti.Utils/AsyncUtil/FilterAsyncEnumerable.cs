using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.AsyncUtil
{
    public class FilterAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IAsyncEnumerable<T> enumerable;
        private readonly Predicate<T> predicate;

        public FilterAsyncEnumerable(IAsyncEnumerable<T> enumerable, Predicate<T> predicate)
        {
            this.enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
            this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator()
            => new AsyncEnumerator(enumerable.GetAsyncEnumerator(), predicate);

        private class AsyncEnumerator : IAsyncEnumerator<T>
        {
            public AsyncEnumerator(IAsyncEnumerator<T> enumerator, Predicate<T> predicate)
            {
                this.enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
                this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            }

            private static Func<T> GetCurrentValue(T value)
                => () => value;

            private static Func<T> GetCurrentThrowInvalidOperationException()
                => () => throw new InvalidOperationException();

            private readonly Predicate<T> predicate;
            private readonly IAsyncEnumerator<T> enumerator;
            private Func<T> GetCurrent = GetCurrentThrowInvalidOperationException();

            public T Current => GetCurrent();

            public void Dispose()
                => enumerator.Dispose();

            public async Task<bool> MoveNextAsync(CancellationToken token)
            {
                
                bool moved = await enumerator.MoveNextAsync(token);
                if(!moved)
                {
                    GetCurrent = GetCurrentThrowInvalidOperationException();
                    return false;
                }

                T value = enumerator.Current;
                if(predicate(value))
                {
                    GetCurrent = GetCurrentValue(value);
                    return true;
                }
                else
                {
                    return await MoveNextAsync(token);
                }
            }

            public void Reset()
            {
                GetCurrent = GetCurrentThrowInvalidOperationException();
                enumerator.Reset();
            }
        }
    }
}
