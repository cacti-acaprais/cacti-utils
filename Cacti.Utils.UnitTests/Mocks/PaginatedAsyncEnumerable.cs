﻿using Cacti.Utils.AsyncUtil;
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

        protected override async Task<T> GetValueAt(int position, CancellationToken token)
        {
            if (position < 0) throw new ArgumentOutOfRangeException(nameof(position));
            if (token == null) throw new ArgumentNullException(nameof(token));
            await CachePagesTo(position, token);

            return values[position];
        }

        protected override async Task<bool> IsValueAt(int position, CancellationToken token)
        {
            if (position < 0) throw new ArgumentOutOfRangeException(nameof(position));
            if (token == null) throw new ArgumentNullException(nameof(token));
            await CachePagesTo(position, token);

            return values.Count > position;
        }

        private async Task CachePagesTo(int position, CancellationToken token)
        {
            while (values.Count <= position && !complete)
            {
                IEnumerable<T> page = await paginatedContext.GetPage((values.Count / pageSize) + 1, pageSize, token);
                
                values.AddRange(page);

                if (page.Count() < pageSize)
                {
                    complete = true;
                }
            }
        }

    }
}
