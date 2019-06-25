using System;
using Sedio.Execution;

namespace Sedio.Web.Execution.Rest
{
    public sealed class ValueOrError<T>
        where T : class
    {
        private readonly T? value;

        internal ValueOrError(T? value, ExecutionErrorCollection errors)
        {
            this.value = value;
            Errors = errors;
        }

        public ExecutionErrorCollection Errors { get; }

        public T Value => value ?? throw new InvalidOperationException("No value present");
    }
}