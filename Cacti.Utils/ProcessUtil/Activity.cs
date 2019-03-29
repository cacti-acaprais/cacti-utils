using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.ProcessUtil
{
    public class Activity<TResult, TData> : IActivity<TResult, TData>
    {
        private readonly Func<TData, CancellationToken, Task<TResult>> _action;

        public Activity(Func<TData, TResult> action)
            : this((data, cancellationToken) =>
            {
                try
                {
                    TResult result = action(data);
                    return Task.FromResult(result);
                }
                catch(Exception exception)
                {
                    return Task.FromException<TResult>(exception);
                }
            })
        {

        }

        public Activity(Func<TData, Task<TResult>> action)
            :this((data, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return action(data);
            })
        {

        }

        public Activity(Func<TData, CancellationToken, Task<TResult>> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public Task<TResult> Execute(TData data, CancellationToken cancellationToken)
        {
            return _action(data, cancellationToken);
        }
    }
}
