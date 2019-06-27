using System;
using System.Net.Http;
using Newtonsoft.Json;
using Refit;
using Sedio.Client.Infrastructure.Http;
using Sedio.Contracts.Serialization;

namespace Sedio.Client
{
    public sealed class SedioClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly JsonSerializerSettings serializerSettings;
        private readonly RefitSettings refitSettings;

        public SedioClient(
            string hostName = "sedio.disc",
            bool useHttps = true,
            int? port = null,
            bool isDebug = true)
        {
            var protocol = useHttps ? "https" : "http";
            var suffix = useHttps ? 
                ((port == 443) ? string.Empty : ":" + port.ToString()) :
                ((port == 80) ? string.Empty : ":" + port.ToString());

            var baseUrl = $"{protocol}://{hostName}{suffix}";

            this.httpClientFactory = new HttpClientFactory();
            this.httpClientFactory.Register("sedio", builder =>
                {
                    builder.ConfigureHttpClient(client =>
                    {
                        client.BaseAddress = new Uri(baseUrl);
                    });

                    builder.ConfigureHttpMessageHandlerBuilder(messageHandler =>
                    {
                        
                    });
                });

            this.serializerSettings = new JsonSerializerSettings();
            new DefaultJsonSerializationConfiguration(!isDebug).ApplySettings(serializerSettings);

            this.refitSettings = new RefitSettings()
            {
                Buffered = true,
                ContentSerializer = new JsonContentSerializer(serializerSettings)
            };
        }
    }
}