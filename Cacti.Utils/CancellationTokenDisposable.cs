using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Cacti.Utils
{
    public class CancellationTokenDisposable : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        public CancellationTokenDisposable()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationToken Token
            => cancellationTokenSource.Token;

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }
}
