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

#pragma warning disable 1998
        protected override async Task<Response> OnExecute(ExecutionContext context, Request request)
#pragma warning restore 1998
        {
            var branch = context.Branch();

            using var transaction = branch.BeginTransaction();

            var serviceNode = new ServiceNode()
            {
                Id = NodeId.Create<ServiceNode>(request.Id),
                ServiceId = request.Id,
                Metadata = NodeMetadata.Create(context.TimeProvider())
            };

            if (branch.Services.PutIfAbsent(serviceNode.Id, serviceNode))
            {
                transaction.Commit();

                return new Response()
                {
                    Service = serviceNode.ToOutput()
                };
            }

            throw new ExecutionException(ExecutionErrorType.Conflict,$"Service {request.Id} already exist");
        }
    }
}