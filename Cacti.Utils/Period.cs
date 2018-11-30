using System;
using System.Collections.Generic;
using System.Text;

namespace Cacti.Utils
{
    public class Period : AbstractRange<Period, DateTimeOffset>
    {
        public Period(DateTimeOffset start, DateTimeOffset end) 
            : base(start, end)
        {
        }
    }
}
