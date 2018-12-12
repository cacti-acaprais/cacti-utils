using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cacti.Utils.AsyncUtil
{
    /// <summary>
    /// Readers wait for writing opeation to complete.
    /// Writers wait for reading and writing operations to complete.
    /// </summary>
    public class AsyncReadWriteLocker
    {
        private readonly AsyncLocker _asyncLocker = new AsyncLocker();
        private readonly Queue<Task> _readingTasks = new Queue<Task>();

        /// <summary>
        /// Add a reading lock. Wait for writing operation to complete.
        /// </summary>
        /// <returns>Disposable reading lock which will be released on dispose.</returns>
        public async Task<IDisposable> ReadLockAsync()
        {
            using (await _asyncLocker.LockAsync())
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                _readingTasks.Enqueue(taskCompletionSource.Task);

                return new ReadingReleaser(() => taskCompletionSource.SetResult(null));
            }
        }

        /// <summary>
        /// Add a writing lock. Wait for writing and reading operations to complete.
        /// </summary>
        /// <returns>Disposable writing lock which will be released on dispose.</returns>
        public async Task<IDisposable> WriteLockAsync()
        {
            IDisposable releaser = await _asyncLocker.LockAsync();

            await Task.WhenAll(_readingTasks);
            _readingTasks.Clear();

            return releaser;
        }

        private class ReadingReleaser : IDisposable
        {
            private readonly Action _onDispose;
            private bool _released = false;

            public ReadingReleaser(Action onDispose)
            {
                _onDispose = onDispose;
            }

            public void Dispose()
            {
                if(!_released)
                {
                    _released = true;
                    _onDispose();
                }
            }
        }
    }
}
