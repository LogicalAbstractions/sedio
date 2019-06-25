using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sedio.Contracts;
using Sedio.Core.Collections.Paging;
using Sedio.Execution;
using Sedio.Logic.Execution.Branches;

namespace Sedio.Server.Api.Rest
{
    [ApiController]
    [Route("api")]
    public sealed class BranchesController : ResourceController
    {
        [HttpGet]
        public async Task<ActionResult<PagingResult<BranchOutput>>> Get()
        {
            return await Execute(new ListBranchesController.Request())
                .Pass.ToResult();
        }

        [HttpGet("{branchId}")]
        public async Task<ActionResult<BranchOutput>> Get(string branchId)
        {
            return await Execute(new GetBranchController.Request()
            {
                Id = branchId
            }).Transform(t => t.Branch).ToResult();
        }

        [HttpPut("{branchId}")]
        public async Task<ActionResult<BranchOutput>> Create(string branchId,BranchInput input)
        {
            return await Execute(new CreateBranchController.Request()
            {
                Id = branchId,
                SourceId = input?.SourceId
            }).Transform(response => response.Branch)
                .ToResult((response, controller) => 
                CreatedAtAction("Get",new {branchId},response));
        }

        [HttpDelete("{branchId}")]
        public Task<ActionResult> Delete(string branchId)
        {
            return Execute(new DeleteBranchController.Request()
            {
                Id = branchId
            }).Pass.ToResult(((response, controller) => new NoContentResult()));
        }
    }
}