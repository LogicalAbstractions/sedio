using System;
using System.Threading.Tasks;

namespace Sedio.Execution
{
    public abstract class ExecutionController<TRequest,TResponse> : IExecutionController
        where TRequest : IExecutionRequest<TResponse>
        where TResponse : class,IExecutionResponse
    {
        public bool CanExecute(ExecutionContext context) => context.Request is TRequest;

        public async Task Execute(ExecutionContext context)
        {
            context.Response = await OnExecute(context, (TRequest) context.Request);
        }

        protected abstract Task<TResponse> OnExecute(ExecutionContext context, TRequest request);
    }
}