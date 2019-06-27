using Newtonsoft.Json;
using Refit;
using Sedio.Client.Infrastructure.Http;

namespace Sedio.Client.Infrastructure
{
    public abstract class ResourceClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly RefitSettings refitSettings;

        protected ResourceClient(IHttpClientFactory httpClientFactory, RefitSettings refitSettings)
        {
            this.httpClientFactory = httpClientFactory;
            this.refitSettings = refitSettings;
        }

        protected T GetClient<T>()
        {
            return RestService.For<T>(httpClientFactory.CreateClient("sedio"), refitSettings);
        }
    }
}