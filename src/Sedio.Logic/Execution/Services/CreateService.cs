using System;
using System.Threading.Tasks;
using Sedio.Contracts;
using Sedio.Core.Data;
using Sedio.Execution;
using Sedio.Logic.Data;
using Sedio.Logic.Data.Components;

namespace Sedio.Logic.Execution.Services
{
    public sealed class CreateServiceController : 
        ExecutionController<CreateServiceController.Request,CreateServiceController.Response>
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
            var branch = await context.Branch().ConfigureAwait(false);

            using var transaction = branch.BeginTransaction();

            var serviceNode = new ServiceNode()
            {
                Id = NodeId.Create<ServiceNode>(request.Id),
                ServiceId = request.Id,
                Metadata = NodeMetadata.Create(context.TimeProvider())
            };

            if (await branch.Services.PutIfAbsentAsync(serviceNode.Id, serviceNode)
                .ConfigureAwait(false))
            {
                await transaction.CommitAsync().ConfigureAwait(false);

                return new Response()
                {
                    Service = serviceNode.ToOutput()
                };
            }

            throw new ExecutionException(ExecutionErrorType.Conflict,$"Service {request.Id} already exist");
        }
    }
}