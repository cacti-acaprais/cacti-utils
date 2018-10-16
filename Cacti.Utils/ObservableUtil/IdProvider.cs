using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Cacti.Utils.ObservableUtil
{
    public class IdProvider
    {
        public IdProvider()
            : this(0)
        {

        }

        public IdProvider(long start)
        {
            id = start;
        }

        private long id = 0;
        public long Get()
            => Interlocked.Increment(ref id);
    }
}
