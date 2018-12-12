using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cacti.Utils
{
    public class Dispose : IDisposable
    {
        private readonly IDisposable[] disposables;

        public Dispose(params IDisposable[] disposables)
        {
            if (disposables?.Any() != true) throw new ArgumentNullException(nameof(disposables));

            this.disposables = disposables;
        }

        void IDisposable.Dispose()
        {
            foreach(IDisposable disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
