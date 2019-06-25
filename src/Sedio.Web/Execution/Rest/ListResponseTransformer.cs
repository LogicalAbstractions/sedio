using System;
using System.Linq;
using System.Threading.Tasks;
using Sedio.Core.Collections.Paging;
using Sedio.Execution;

namespace Sedio.Web.Execution.Rest
{
    public sealed class ListResponseTransformer<TResponse, TItem>
        where TResponse : class,IPagingExecutionResponse<TItem>
    {
        private readonly Task<ExecutionResult<TResponse>> source;
        private readonly ExecutionHostController controller;

        internal ListResponseTransformer(Task<ExecutionResult<TResponse>> source, ExecutionHostController controller)
        {
            this.source = source;
            this.controller = controller;
        }

        public ActionResultTransformer<PagingResult<TItem>> Pass => Transform<TItem>(i => i);

        public ActionResultTransformer<PagingResult<T>> Transform<T>(Func<TItem, T> transformFunc)
            
        {
            async Task<ValueOrError<PagingResult<T>>> TransformWrapper()
            {
                var response = await source.ConfigureAwait(false);

                if (response.Errors.Any())
                {
                    return new ValueOrError<PagingResult<T>>(null, response.Errors);
                }

                return new ValueOrError<PagingResult<T>>(response.Response.Items.Map(transformFunc),response.Errors);
            }

            return new ActionResultTransformer<PagingResult<T>>(TransformWrapper(), controller);
        }
    }
}