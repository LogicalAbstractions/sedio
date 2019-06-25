using System.Threading.Tasks;
using Sedio.Contracts;
using Sedio.Execution;
using Sedio.Logic.Data;

namespace Sedio.Logic.Execution.Branches
{
    public sealed class CreateBranchController : ExecutionController<CreateBranchController.Request,CreateBranchController.Response>
    {
        public sealed class Request : IExecutionRequest<Response>
        {
            public string Id { get; set; } = string.Empty;

            public string? SourceId { get; set; }
        }

        public sealed class Response : IExecutionResponse
        {
            public BranchOutput Branch { get; set; } = new BranchOutput();
        }

        protected override async Task<Response> OnExecute(ExecutionContext context, Request request)
        {
            var branchProvider = context.BranchProvider();

            if (!branchProvider.Exists(request.Id))
            {
                branchProvider.Create(request.Id);

                if (request.SourceId != null)
                {
                    if (branchProvider.Exists(request.SourceId))
                    {
                        await branchProvider.Copy(request.SourceId, request.Id).ConfigureAwait(false);
                    }
                    else
                    {
                        throw new ExecutionException(ExecutionErrorType.NotFound,$"Source branch {request.SourceId} not found");
                    }
                }

                return new Response() {Branch = new BranchOutput()
                {
                    Id = request.Id
                }};
            }

            throw new ExecutionException(ExecutionErrorType.Conflict,$"Branch {request.Id} already exists");
        }
    }
}