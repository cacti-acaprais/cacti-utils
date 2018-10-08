using Cacti.Utils.JobUtil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.StepUtil
{
    public class Step<TIn, TOut> : IStep<TIn, TOut>
    {
        private readonly Func<TIn, CancellationToken, Task<TOut>> action;

        public Step(Func<TIn, TOut> action)
            : this((@in, token) =>
            {
                TOut result = action(@in);
                return Task.FromResult(result);
            })
        { }

        public Step(Func<TIn, Task<TOut>> action)
            : this(async (@in, token) =>
            {
                if (token.IsCancellationRequested)
                    throw new OperationCanceledException();

                return await action(@in);
            })
        { }

        public Step(Func<TIn, CancellationToken, Task<TOut>> action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public Task<TOut> Execute(TIn @in, CancellationToken token)
            => action(@in, token);
    }
}
