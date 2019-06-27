using System.Collections.Specialized;
using System.Data;
using System.Threading.Tasks;
using Sedio.Core.Data;
using Sedio.Execution;
using Sedio.Logic.Data;

namespace Sedio.Logic.Execution.Services
{
    public sealed class DeleteServiceController : 
        ExecutionController<DeleteServiceController.Request,DeleteServiceController.Response>
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
            var branch = await context.Branch().ConfigureAwait(false);

            if (await branch.Services.RemoveAsync(NodeId.Create<ServiceNode>(request.Id))
                .ConfigureAwait(false))
            {
                return new Response()
                {
                    Id = request.Id
                };
            }

            throw new ExecutionException(ExecutionErrorType.NotFound,$"Service {request.Id} does not exist");
        }
    }
}