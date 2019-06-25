using System;
using System.Collections.Generic;
using System.Linq;

namespace Sedio.Core.Composition
{
    public static class Pipeline<TIn,TOut>
    {
        public delegate TOut InvocationDelegate(TIn input);
        public delegate TOut NextDelegate();
        public delegate TOut StepDelegate(TIn input, NextDelegate next);

        public static InvocationDelegate Compile(IEnumerable<StepDelegate> steps,Func<TOut> initialOutProvider)
        {
            TOut Terminator(TIn input) => initialOutProvider.Invoke();

            return steps.Reverse().Aggregate((InvocationDelegate) Terminator, 
                (current, step) => (input) => step.Invoke(input, () => current.Invoke(input)));
        } 
    }
    
    public static class Pipeline<TIn>
    {
        public delegate void InvocationDelegate(TIn input);
        public delegate void NextDelegate();
        public delegate void StepDelegate(TIn input, NextDelegate next);

        public static InvocationDelegate Compile(IEnumerable<StepDelegate> steps)
        {
            void Terminator(TIn input) {}

            return steps.Reverse().Aggregate((InvocationDelegate) Terminator, 
                (current, step) => (input) => step.Invoke(input, () => current.Invoke(input)));
        } 
    }
}
