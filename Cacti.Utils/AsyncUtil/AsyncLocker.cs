using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.AsyncUtil
{
    public class AsyncLocker
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(initialCount: 1);

        public async Task<IDisposable> LockAsync()
        {
            await _semaphoreSlim.WaitAsync();

            return new Releaser(() => _semaphoreSlim.Release());
        }

        private class Releaser : IDisposable
        {
            private readonly Action _onRelease;
            private bool _released = false;

            public Releaser(Action onRelease)
            {
                _onRelease = onRelease;
            }

            public void Dispose()
            {
                if(!_released)
                {
                    _released = true;
                    _onRelease();
                }
            }
        }
    }
}
