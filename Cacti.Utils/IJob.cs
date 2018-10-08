using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils
{
    public interface IJob
    {
        Task Execute(CancellationToken token);
    }
}
