using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sedio.Core.Collections.Paging;
using Sedio.Execution;
using Sedio.Web.Collections;

namespace Sedio.Web.Execution.Rest
{
    public abstract class ExecutionHostController : ControllerBase
    {
        protected SingleResponseTransformer<TResponse> Execute<TResponse>(
            IExecutionRequest<TResponse> request)
            where TResponse : class,IExecutionResponse
        {
            return new SingleResponseTransformer<TResponse>(ExecuteTyped(request),this);
        }

        protected ListResponseTransformer<TResponse, TItem> Execute<TResponse, TItem>(
            IPagingExecutionRequest<TResponse, TItem> request)
            where TResponse : class,IPagingExecutionResponse<TItem>
        {
            return new ListResponseTransformer<TResponse, TItem>(ExecuteTyped(request),this);
        }

        private async Task<ExecutionResult<TResponse>> ExecuteTyped<TResponse>(IExecutionRequest<TResponse> request)
            where TResponse : class,IExecutionResponse
        {
            var untypedResult = await ExecuteCore(request).ConfigureAwait(false);

            return untypedResult.Cast<TResponse>();
        }

        private async Task<ExecutionResult> ExecuteCore(IExecutionRequest request)
        {
            var executionEngine = HttpContext.RequestServices.GetRequiredService<IExecutionEngine>();

            if (request is IPagingExecutionRequest listRequest)
            {
                listRequest.PagingParameters = PagingParameters.Create(
                    HttpContext.Request.Query.GetIntValueNullable("offset"),
                    HttpContext.Request.Query.GetIntValueNullable("limit")
                    );
            }

            var response = await executionEngine.Execute( 
                    request, 
                    OnGetExecutionItems(),
                    HttpContext.RequestAborted)
                .ConfigureAwait(false);

            return response;
        }

        protected abstract IReadOnlyDictionary<string, object> OnGetExecutionItems();
    }
}