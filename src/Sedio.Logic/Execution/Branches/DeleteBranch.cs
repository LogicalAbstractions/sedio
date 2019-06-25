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

#pragma warning disable 1998
        protected override async Task<Response> OnExecute(ExecutionContext context, Request request)
#pragma warning restore 1998
        {
            var branchProvider = context.BranchProvider();

            if (branchProvider.Exists(request.Id))
            {
                branchProvider.Delete(request.Id);

                return new Response()
                {
                    Id = request.Id
                };
            }

            throw new ExecutionException(ExecutionErrorType.NotFound,$"Branch {request.Id} does not exist");
        }
    }
}