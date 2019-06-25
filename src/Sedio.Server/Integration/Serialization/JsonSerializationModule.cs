using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sedio.Contracts.Serialization;
using Sedio.Web.Serialization;

namespace Sedio.Server.Integration.Serialization
{
    public sealed class JsonSerializationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var hostEnvironment = c.Resolve<IHostEnvironment>();

                return new DefaultJsonSerializationConfiguration(hostEnvironment.IsProduction());
            }).As<IJsonSerializationConfiguration>().SingleInstance();

            builder.Register(c =>
            {
                var configuration = c.Resolve<IJsonSerializationConfiguration>();
                return new JsonOptionsConfiguration(o => configuration.ApplySettings(o.SerializerSettings));
            }).As<IConfigureOptions<MvcNewtonsoftJsonOptions>>().SingleInstance();

            builder.Register(c =>
            {
                var settings = new JsonSerializerSettings();
                c.Resolve<IJsonSerializationConfiguration>().ApplySettings(settings);
                return settings;
            }).As<JsonSerializerSettings>().SingleInstance();

            builder.Register(c => JsonSerializer.Create(c.Resolve<JsonSerializerSettings>())).As<JsonSerializer>()
                .SingleInstance();
        }
    }
}