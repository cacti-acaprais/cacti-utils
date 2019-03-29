using System;
using System.Collections.Generic;
using System.Text;

namespace Cacti.Utils.ProcessUtil
{
    public static class ProcessBuilder
    {
        public static IActivity<TOtherResult, TData> Then<TResult, TData, TOtherResult>(this IActivity<TResult, TData> activity, IActivity<TOtherResult, TResult> otherActivity)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));
            if (otherActivity == null) throw new ArgumentNullException(nameof(otherActivity));

            return new Activity<TOtherResult, TData>(async (data, cancellationToken) =>
            {
                TResult result = await activity.Execute(data, cancellationToken).ConfigureAwait(false);
                return await otherActivity.Execute(result, cancellationToken).ConfigureAwait(false);
            });
        }

        public static IActivity<TResult, TData> While<TResult, TData>(
            this IActivity<TResult, TData> activity, 
            Predicate<TResult> predicate, 
            Func<(TResult result, TData data), (TResult result, TData data), (TResult result, TData data)> accumulator)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));

            return new Activity<TResult, TData>(async (data, token) =>
            {
                (TResult, TData) accumulatorValue = (default(TResult), data);

                do
                {
                    TResult result = await activity.Execute(accumulatorValue.Item2, token).ConfigureAwait(false);
                    accumulatorValue = accumulator(accumulatorValue, (result, accumulatorValue.Item2));
                }
                while (predicate(accumulatorValue.Item1));

                return accumulatorValue.Item1;
            });
        }

        public static Func<(T result, T data), (T result, T data), (T result, T data)> SetLastResultToDataAccumulator<T>(this IActivity<T, T> activity)
            => (previous, current) => current.SetLastResultToData();

        public static (T result, T data) SetLastResultToData<T>(this (T result, T data) param)
            => (result: param.result, data: param.result);
    }
}
