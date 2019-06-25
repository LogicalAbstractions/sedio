using Newtonsoft.Json;

namespace Sedio.Contracts.Serialization
{
    public interface IJsonSerializationConfiguration
    {
        void ApplySettings(JsonSerializerSettings settings);
    }
}