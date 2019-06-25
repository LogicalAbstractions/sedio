using Microsoft.Extensions.Hosting;
using Sedio.Core.Application;

namespace Sedio.Web.Application
{
    public sealed class WebApplication : IApplication
    {
        private readonly IHostEnvironment hostEnvironment;

        public WebApplication(IHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        public string RootPath => hostEnvironment.ContentRootPath;

        public bool IsProduction => hostEnvironment.IsProduction();
    }
}