using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Sedio.Contracts.Serialization
{
    public sealed class DefaultJsonSerializationConfiguration : IJsonSerializationConfiguration
    {
        private readonly bool isProduction;

        public DefaultJsonSerializationConfiguration(bool isProduction)
        {
            this.isProduction = isProduction;
        }

        public void ApplySettings(JsonSerializerSettings settings)
        {
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Converters.Add(new StringEnumConverter());
            settings.Formatting = isProduction ? Formatting.None : Formatting.Indented;
        }
    }
}