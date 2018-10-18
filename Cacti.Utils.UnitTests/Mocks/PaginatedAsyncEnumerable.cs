using Cacti.Utils.AsyncUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.UnitTests.Mocks
{
    public class PaginatedAsyncEnumerable<T> : AbstractAsyncEnumerable<T>
    {
        private readonly PaginatedContext<T> paginatedContext;
        private readonly int pageSize;
        private readonly List<T> values = new List<T>();
        private bool complete = false;

        public PaginatedAsyncEnumerable(int pageSize, PaginatedContext<T> paginatedContext)
        {
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

            this.pageSize = pageSize;
            this.paginatedContext = paginatedContext ?? throw new ArgumentNullException(nameof(paginatedContext));
        }

        protected override Task<T> GetValueAt(int position, CancellationToken token)
        {
            if (position < 0) throw new ArgumentOutOfRangeException(nameof(position));
            if (token == null) throw new ArgumentNullException(nameof(token));
            CachePagesTo(position);

            return Task.FromResult(values[position]);
        }

        protected override Task<bool> IsValueAt(int position, CancellationToken token)
        {
            if (position < 0) throw new ArgumentOutOfRangeException(nameof(position));
            if (token == null) throw new ArgumentNullException(nameof(token));
            CachePagesTo(position);

            return Task.FromResult(values.Count > position);
        }

        private void CachePagesTo(int position)
        {
            while (values.Count <= position && !complete)
            {
                IEnumerable<T> page = paginatedContext.GetPage((values.Count / pageSize) + 1, pageSize);
                
                values.AddRange(page);

                if (page.Count() < pageSize)
                {
                    complete = true;
                }
            }
        }

    }
}
