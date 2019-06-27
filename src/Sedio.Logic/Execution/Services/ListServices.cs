using System.Threading.Tasks;
using Apache.Ignite.Linq;
using Sedio.Contracts;
using Sedio.Core.Collections.Paging;
using Sedio.Execution;
using Sedio.Logic.Data;

namespace Sedio.Logic.Execution.Services
{
    public sealed class ListServicesController : 
        ExecutionController<ListServicesController.Request,PagingExecutionResponse<ServiceOutput>>
    {
        public sealed class Request : IPagingExecutionRequest<PagingExecutionResponse<ServiceOutput>,ServiceOutput>
        {
            public PagingParameters PagingParameters { get; set; } = PagingParameters.Create();
        }

#pragma warning disable 1998
        protected override async Task<PagingExecutionResponse<ServiceOutput>> OnExecute(
            ExecutionContext context, 
            Request request)
#pragma warning restore 1998
        {
            var branch = await context.Branch().ConfigureAwait(false);

            var items = branch.Services.AsCacheQueryable().ApplyPaging(request.PagingParameters);

            return new PagingExecutionResponse<ServiceOutput>()
            {
                Items = items.Map(e => e.Value.ToOutput())
            };
        }
    }
}