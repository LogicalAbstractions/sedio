using System.Threading.Tasks;
using Sedio.Contracts;
using Sedio.Core.Data;
using Sedio.Execution;
using Sedio.Ignite;
using Sedio.Logic.Data;
using Sedio.Logic.Data.Components;

namespace Sedio.Logic.Execution.Services
{
    public sealed class GetServiceController : ExecutionController<GetServiceController.Request,GetServiceController.Response>
    {
        public sealed class Request : IExecutionRequest<Response>
        {
            public string Id { get; set; } = string.Empty;
        }

        public sealed class Response : IExecutionResponse
        {
            public ServiceOutput Service { get; set; } = new ServiceOutput();
        }

        protected override async Task<Response> OnExecute(ExecutionContext context, Request request)
        {
            var branch = context.Branch();

            var serviceNode = await branch.Services.GetAsyncOrDefault(NodeId.Create<ServiceNode>(request.Id))
                .ConfigureAwait(false);

            if (serviceNode != null)
            {
                return new Response()
                {
                    Service = serviceNode.ToOutput()
                };
            }

            throw new ExecutionException(ExecutionErrorType.NotFound,$"Service {request.Id} does not exist");
        }
    }
}