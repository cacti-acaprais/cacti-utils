using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.AsyncUtil
{
    public class CompositeAsyncEnumerable<TIn, TOut> : IAsyncEnumerable<TOut>
    {
        private readonly IAsyncEnumerable<TIn> enumerable;
        private readonly Func<TIn, TOut> onCurrent;
        private readonly Func<CancellationToken, Func<Task<bool>>, Task<bool>> onMoveNext;

        public CompositeAsyncEnumerable(IAsyncEnumerable<TIn> enumerable, Func<TIn, TOut> onCurrent)
            : this(enumerable, onCurrent, async (token, hasNext) => await hasNext())
        {

        }

        public CompositeAsyncEnumerable(IAsyncEnumerable<TIn> enumerable, Func<TIn, TOut> onCurrent, Func<CancellationToken, Func<Task<bool>>, Task<bool>> onMoveNext)
        {
            this.enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
            this.onCurrent = onCurrent ?? throw new ArgumentNullException(nameof(onCurrent));
            this.onMoveNext = onMoveNext ?? throw new ArgumentNullException(nameof(onMoveNext));
        }

        public IAsyncEnumerator<TOut> GetAsyncEnumerator()
        {
            return new AsyncEnumerator(enumerable.GetAsyncEnumerator(), onCurrent, onMoveNext);
        }

        private class AsyncEnumerator : IAsyncEnumerator<TOut>
        {
            public AsyncEnumerator(IAsyncEnumerator<TIn> enumerator, Func<TIn, TOut> onCurrent, Func<CancellationToken, Func<Task<bool>>, Task<bool>> onMoveNext)
            {
                this.enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
                this.onCurrent = onCurrent ?? throw new ArgumentNullException(nameof(onCurrent));
                this.onMoveNext = onMoveNext ?? throw new ArgumentNullException(nameof(onMoveNext));
            }

            private static Func<TOut> GetCurrentValue(TOut value)
                => () => value;

            private readonly Func<TIn, TOut> onCurrent;
            private readonly Func<CancellationToken, Func<Task<bool>>, Task<bool>> onMoveNext;
            private readonly IAsyncEnumerator<TIn> enumerator;

            public TOut Current => onCurrent(enumerator.Current);

            public void Dispose()
                => enumerator.Dispose();

            public async Task<bool> MoveNextAsync(CancellationToken token)
                => await onMoveNext(token, async () => await enumerator.MoveNextAsync(token));

            public void Reset()
                => enumerator.Reset();
        }
    }
}
