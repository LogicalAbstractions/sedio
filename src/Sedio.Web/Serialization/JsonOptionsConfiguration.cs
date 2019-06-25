using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Sedio.Web.Serialization
{
    public sealed class JsonOptionsConfiguration : IConfigureOptions<MvcNewtonsoftJsonOptions>
    {
        private readonly Action<MvcNewtonsoftJsonOptions> configureAction;

        public JsonOptionsConfiguration(Action<MvcNewtonsoftJsonOptions> configureAction)
        {
            this.configureAction = configureAction;
        }

        public void Configure(MvcNewtonsoftJsonOptions options)
        {
            configureAction.Invoke(options);
        }
    }
}