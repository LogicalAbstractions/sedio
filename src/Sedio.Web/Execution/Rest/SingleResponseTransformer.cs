using System;
using System.Linq;
using System.Threading.Tasks;
using Sedio.Execution;

namespace Sedio.Web.Execution.Rest
{
    public sealed class SingleResponseTransformer<TResponse> 
        where TResponse : class,IExecutionResponse
    {
        private readonly Task<ExecutionResult<TResponse>> source;
        private readonly ExecutionHostController controller;

        internal SingleResponseTransformer(Task<ExecutionResult<TResponse>> source, ExecutionHostController controller)
        {
            this.source = source;
            this.controller = controller;
        }

        public ActionResultTransformer<TResponse> Pass => Transform<TResponse>(r => r);

        public ActionResultTransformer<T> Transform<T>(Func<TResponse, T> transformFunc)
            where T : class 
        {
            async Task<ValueOrError<T>> TransformWrapper()
            {
                var response = await source.ConfigureAwait(false);

                if (response.Errors.Any())
                {
                    return new ValueOrError<T>(null,response.Errors);
                }

                return new ValueOrError<T>(transformFunc.Invoke(response.Response), response.Errors);
            }

            return new ActionResultTransformer<T>(TransformWrapper(),controller);
        }
    }
}