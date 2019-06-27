using System.Threading.Tasks;
using Sedio.Contracts;
using Sedio.Execution;
using Sedio.Logic.Data;

namespace Sedio.Logic.Execution.Branches
{
    public sealed class GetBranchController : ExecutionController<GetBranchController.Request,GetBranchController.Response>
    {
        public sealed class Request : IExecutionRequest<Response>
        {
            public string Id { get; set; } = string.Empty;
        }

        public sealed class Response : IExecutionResponse
        {
            public BranchOutput Branch { get; set; } = new BranchOutput();
        }

        protected override async Task<Response> OnExecute(ExecutionContext context, Request request)
        {
            var branchProvider = context.BranchProvider();

            if (await branchProvider.Exists(request.Id).ConfigureAwait(false))
            {
                return new Response()
                {
                    Branch = new BranchOutput()
                    {
                        Id = request.Id
                    }
                };
            }

            throw new ExecutionException(ExecutionErrorType.NotFound,$"Branch {request.Id} does not exist");
        }
    }
}