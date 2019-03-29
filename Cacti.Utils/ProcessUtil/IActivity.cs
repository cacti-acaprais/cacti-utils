using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.ProcessUtil
{
    public interface IActivity<TResult, TData>
    {
        Task<TResult> Execute(TData data, CancellationToken cancellationToken);
    }
}
