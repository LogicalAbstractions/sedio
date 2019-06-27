using System.Threading.Tasks;
using Refit;
using Sedio.Client.Infrastructure;
using Sedio.Client.Infrastructure.Http;
using Sedio.Contracts;
using Sedio.Core.Collections.Paging;

namespace Sedio.Client
{
    public sealed class BranchesClient : ResourceClient
    {
        internal interface IBranchesOperations
        {
            [Get("/api/{branchId}")]
            Task<BranchOutput> Get(string branchId);

            [Get("/api")]
            Task<PagingResult<BranchOutput>> GetAll([Query] PagingParameters pagingParameters);

            [Put("/api/{branchId}")]
            Task<BranchOutput> Create(string branchId,[Body] BranchInput branchInput);

            [Delete("/api/{branchId}")]
            Task Delete(string branchId);
        }

        private readonly IBranchesOperations operations;

        public BranchesClient(IHttpClientFactory httpClientFactory, 
            RefitSettings refitSettings) 
            : base(httpClientFactory, refitSettings)
        {
            this.operations = GetClient<IBranchesOperations>();
        }

        public Task<BranchOutput> Get(string branchId)
        {
            return operations.Get(branchId);
        }
    }
}