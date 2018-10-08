using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.StepUtil
{
    public interface IStep<TIn, TOut>
    {
        Task<TOut> Execute(TIn @in, CancellationToken token);
    }
}
