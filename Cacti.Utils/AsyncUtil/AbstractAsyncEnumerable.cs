using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.AsyncUtil
{
    public abstract class AbstractAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        protected abstract Task<T> GetValueAt(int position, CancellationToken token);
        protected abstract Task<bool> IsValueAt(int position, CancellationToken token);

        public IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new AsyncEnumerator(GetValueAt, IsValueAt);
        }

        private class AsyncEnumerator : IAsyncEnumerator<T>
        {
            public AsyncEnumerator(Func<int, CancellationToken, Task<T>> getValueAt, Func<int, CancellationToken, Task<bool>> isValueAt)
            {
                this.GetValueAt = getValueAt ?? throw new ArgumentNullException(nameof(getValueAt));
                this.IsValueAt = isValueAt ?? throw new ArgumentNullException(nameof(isValueAt)); 
            }

            private static Func<T> getCurrentInvalidOperation
                => () => throw new InvalidOperationException();

            private static Func<T> getCurrentValue(T value)
                => () => value;

            private int position = 0;
            private readonly Func<int, CancellationToken, Task<T>> GetValueAt;
            private readonly Func<int, CancellationToken, Task<bool>> IsValueAt;
            private Func<T> GetCurrent = getCurrentInvalidOperation;

            public T Current => GetCurrent();

            public async Task<bool> MoveNextAsync(CancellationToken token)
            {
                if(await IsValueAt(position, token))
                {
                    T value = await GetValueAt(position, token);
                    GetCurrent = getCurrentValue(value);
                    position++;

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                position = 0;
            }

            public void Dispose()
            {
                
            }
        }
    }
}
