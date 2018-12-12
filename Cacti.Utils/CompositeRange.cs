using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cacti.Utils
{
    public class CompositeRange<Y, T> : IEnumerable<Y>
        where Y : AbstractRange<Y, T>
        where T : IComparable<T>
    {
        private readonly List<Y> ranges;

        public CompositeRange(IEnumerable<Y> ranges)
        {
            if (ranges == null) throw new ArgumentNullException(nameof(ranges));

            ranges = new List<Y>(ranges);
        }

        public IEnumerator<Y> GetEnumerator()
            => ranges.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ranges.GetEnumerator();
    }
}
