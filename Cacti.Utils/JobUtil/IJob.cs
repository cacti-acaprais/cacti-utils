﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.JobUtil
{
    public interface IJob : IDisposable
    {
        Task Execute(CancellationToken token);
    }
}
