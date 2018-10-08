using System;
using System.Collections.Generic;
using System.Text;

namespace Cacti.Utils.StepUtil
{
    public static class StepExtensions
    {
        public static IStep<TIn,TNextOut> Then<TIn, TOut, TNextOut>(this IStep<TIn, TOut> step, IStep<TOut, TNextOut> nextStep)
        {
            if (step == null) throw new ArgumentNullException(nameof(step));
            if (nextStep == null) throw new ArgumentNullException(nameof(nextStep));

            return new Step<TIn, TNextOut>(async (@in, token) =>
            {
                TOut result = await step.Execute(@in, token);
                return await nextStep.Execute(result, token);
            });
        }

        public static IStep<TIn, TNextOut> Then<TIn, TOut, TNextOut>(this IStep<TIn, TOut> step, Func<TOut, IStep<TOut, TNextOut>> selector)
        {
            if (step == null) throw new ArgumentNullException(nameof(step));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return new Step<TIn, TNextOut>(async (@in, token) =>
            {
                TOut result = await step.Execute(@in, token);
                return await selector(result).Execute(result, token);
            });
        }
    }
}
