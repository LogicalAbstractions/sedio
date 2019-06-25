using System.Threading.Tasks;
using Sedio.Contracts;
using Sedio.Core.Collections.Paging;
using Sedio.Execution;
using Sedio.Ignite;
using Sedio.Logic.Data;

namespace Sedio.Logic.Execution.Branches
{
    public sealed class ListBranchesController : ExecutionController<ListBranchesController.Request,PagingExecutionResponse<BranchOutput>>
    {
        public sealed class Request : IPagingExecutionRequest<PagingExecutionResponse<BranchOutput>,BranchOutput>
        {
            public PagingParameters PagingParameters { get; set; } = PagingParameters.Create();
        }

#pragma warning disable 1998
        protected override async Task<PagingExecutionResponse<BranchOutput>> OnExecute(ExecutionContext context, Request request)
#pragma warning restore 1998
        {
            var branchIds = context.BranchProvider().BranchIds;
            return new PagingExecutionResponse<BranchOutput>()
            {
                Items = branchIds.ApplyPaging(request.PagingParameters).Map(bId => new BranchOutput() { Id = bId})
            };
        }
    }
}