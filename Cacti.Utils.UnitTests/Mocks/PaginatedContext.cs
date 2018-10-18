using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.UnitTests.Mocks
{
    public class PaginatedContext<T>
    {
        private int total;
        private Func<int, T> valueFactory;
        private readonly TimeSpan delay;

        public PaginatedContext(int total, Func<int, T> valueFactory, TimeSpan delay)
        {
            if (total < 0) throw new ArgumentOutOfRangeException(nameof(total));

            this.total = total;
            this.valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            this.delay = delay;
        }

        public async Task<IEnumerable<T>> GetPage(int page, int count, CancellationToken token)
        {
            await Task.Delay(delay, token);
            return GeneratePage(page, count);
        }

        private IEnumerable<T> GeneratePage(int page, int count)
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
