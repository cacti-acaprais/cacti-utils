using System;
using System.Collections.Generic;
using System.Text;

namespace Cacti.Utils.UnitTests.Mocks
{
    public class PaginatedContext<T>
    {
        private int total;
        private Func<int, T> valueFactory;

        public PaginatedContext(int total, Func<int, T> valueFactory)
        {
            if (total < 0) throw new ArgumentOutOfRangeException(nameof(total));

            this.total = total;
            this.valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        }

        public IEnumerable<T> GetPage(int page, int count)
        {
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count));

            int start = (page - 1) * count;
            int end = page * count;

            if (end > total)
                end = total;

            for (int i = start; i < end; i++)
                yield return valueFactory(i);
        }
    }
}
