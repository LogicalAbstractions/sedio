using System;
using System.ComponentModel;

namespace Sedio.Execution
{
    public class ExecutionResult
    {
        private readonly object? response;

        internal ExecutionResult(object? response, ExecutionErrorCollection errors)
        {
            this.response = response;
            Errors = errors;
        }

        public ExecutionErrorCollection Errors { get; }

        public object Response => response ?? throw new InvalidOperationException("No response produced");

        public ExecutionResult<TResponse> Cast<TResponse>()
            where TResponse : class,IExecutionResponse
        {
            return new ExecutionResult<TResponse>(response,Errors);
        }
    }

    public sealed class ExecutionResult<TResponse> : ExecutionResult 
        where TResponse : class,IExecutionResponse
    {
        internal ExecutionResult(object? response, ExecutionErrorCollection errors)
            : base(response,errors)
        {
       
        }

        public new TResponse Response => (TResponse) base.Response;
    }
}