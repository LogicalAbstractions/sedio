using System.Threading.Tasks;
using Sedio.Execution;
using Sedio.Logic.Data;

namespace Sedio.Logic.Execution.Branches
{
    public sealed class DeleteBranchController : ExecutionController<DeleteBranchController.Request,DeleteBranchController.Response>
    {
        public sealed class Request : IExecutionRequest<Response>
        {
            public string Id { get; set; } = string.Empty;
        }

        public sealed class Response : IExecutionResponse
        {
            public string Id { get; set; } = string.Empty;
        }

        protected override async Task<Response> OnExecute(ExecutionContext context, Request request)
        {
            var branchProvider = context.BranchProvider();

            if (await branchProvider.Exists(request.Id).ConfigureAwait(false))
            {
                await branchProvider.Delete(request.Id).ConfigureAwait(false);

                return new Response()
                {
                    Id = request.Id
                };
            }

            throw new ExecutionException(ExecutionErrorType.NotFound,$"Branch {request.Id} does not exist");
        }
    }
}