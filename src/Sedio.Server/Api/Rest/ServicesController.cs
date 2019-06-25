using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sedio.Contracts;
using Sedio.Core.Collections.Paging;
using Sedio.Logic.Execution.Services;
using Sedio.Web.Execution;

namespace Sedio.Server.Api.Rest
{
    [ApiController]
    [Route("api/{branchId}/services")]
    public sealed class ServicesController : ResourceController
    {
        [HttpGet]
        public async Task<ActionResult<PagingResult<ServiceOutput>>> Get()
        {
            return await Execute(new ListServicesController.Request())
                .Pass.ToResult();
        }

        [HttpGet("{serviceId}")]
        public async Task<ActionResult<ServiceOutput>> Get(string serviceId)
        {
            return await Execute(new GetServiceController.Request()
            {
                Id = serviceId
            }).Transform(t => t.Service).ToResult();
        }

        [HttpPut("{serviceId}")]
        public async Task<ActionResult<ServiceOutput>> Create(string serviceId, ServiceInput input)
        {
            return await Execute(new CreateServiceController.Request()
            {
                Id = serviceId
              
            }).Transform(response => response.Service)
            .ToResult((response, controller) =>
                CreatedAtAction("Get", new
                {
                    branchId = BranchId,
                    serviceId = serviceId
                },response));
        }

        [HttpDelete("{serviceId}")]
        public Task<ActionResult> Delete(string serviceId)
        {
            return Execute(new DeleteServiceController.Request()
            {
                Id = serviceId
            }).Pass.ToResult(((response, controller) => new NoContentResult()));
        }
    }
}