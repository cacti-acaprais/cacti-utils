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
        private readonly List<Task> _readingTasks = new List<Task>();

        /// <summary>
        /// Add a reading lock. Wait for writing operation to complete.
        /// </summary>
        /// <returns>Disposable reading lock which will be released on dispose.</returns>
        public async Task<IDisposable> ReadLockAsync(CancellationToken token)
        {
            using (await _asyncLocker.LockAsync(token))
            {
                _readingTasks.RemoveAll(x => x.IsCompleted);
                
                var taskCompletionSource = new TaskCompletionSource<object>();
                _readingTasks.Add(taskCompletionSource.Task);

                return new ReadingReleaser(() => taskCompletionSource.SetResult(null));
            }
        }

        /// <summary>
        /// Add a writing lock. Wait for writing and reading operations to complete.
        /// </summary>
        /// <returns>Disposable writing lock which will be released on dispose.</returns>
        public async Task<IDisposable> WriteLockAsync(CancellationToken token)
        {
            IDisposable releaser = await _asyncLocker.LockAsync(token);

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
