using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sedio.Core.Composition
{
    public static class AsyncPipeline<TIn,TOut>
    {
        public delegate Task<TOut> InvocationDelegate(TIn input);
        public delegate Task<TOut> NextDelegate();
        public delegate Task<TOut> StepDelegate(TIn input, NextDelegate next);

        public static InvocationDelegate Compile(IEnumerable<StepDelegate> steps,Func<TOut> initialOutProvider)
        {
            Task<TOut> Terminator(TIn input) => Task.FromResult(initialOutProvider.Invoke());

            return steps.Reverse().Aggregate((InvocationDelegate) Terminator, 
                (current, step) => (input) => step.Invoke(input, () => current.Invoke(input)));
        } 
    }
    
    public static class AsyncPipeline<TIn>
    {
        public delegate Task InvocationDelegate(TIn input);
        public delegate Task NextDelegate();
        public delegate Task StepDelegate(TIn input, NextDelegate next);

        public static InvocationDelegate Compile(IEnumerable<StepDelegate> steps)
        {
            Task Terminator(TIn input) => Task.CompletedTask;

            return steps.Reverse().Aggregate((InvocationDelegate) Terminator, 
                (current, step) => (input) => step.Invoke(input, () => current.Invoke(input)));
        } 
    }
}
