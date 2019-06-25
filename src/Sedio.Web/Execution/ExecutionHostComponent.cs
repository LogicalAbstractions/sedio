using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sedio.Execution;

namespace Sedio.Web.Execution
{
    public abstract class ExecutionHostComponent : ComponentBase
    {
        [Inject]
        protected IExecutionEngine ExecutionEngine { get; set; } = NullExecutionEngine.Default;

        private async Task<ExecutionResult> ExecuteCore(IExecutionRequest request,CancellationToken cancellationToken = default)
        {
            if (request is IPagingExecutionRequest listRequest)
            {
                //listRequest.Limit = int.Parse(HttpContext.Request.Query.GetValueOrDefault("limit", "8"));
                //listRequest.Offset = int.Parse(HttpContext.Request.Query.GetValueOrDefault("offset", "0"));
            }

            var response = await ExecutionEngine.Execute(
                    request,
                    OnGetExecutionItems(),
                    cancellationToken)
                .ConfigureAwait(false);

            return response;
        }

        protected abstract IReadOnlyDictionary<string, object> OnGetExecutionItems();
    }
}