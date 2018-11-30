using System;
using System.Collections.Generic;
using System.Text;

namespace Cacti.Utils
{
    public abstract class AbstractRange<Y, T>
        where Y : AbstractRange<Y, T>
        where T : IComparable<T>
    {
        public T Start { get; }
        public T End { get; }

        public AbstractRange(T start, T end)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (end == null) throw new ArgumentNullException(nameof(end));

            if (end.CompareTo(start) < 0)
                throw new InvalidOperationException($"{nameof(end)} : {end.ToString()} > {nameof(start)} : {start.ToString()}");

            Start = start;
            End = end;
        }

        public bool Contains(Y other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            return other.Start.CompareTo(Start) >= 0 && other.End.CompareTo(End) <= 0;
        }

        public bool Intersect(Y other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            return other.Start.CompareTo(End) <= 0 && other.End.CompareTo(Start) >= 0;
        }
    }
}
