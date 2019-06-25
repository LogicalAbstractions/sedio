using Sedio.Contracts.Components;

namespace Sedio.Contracts
{
    public class ServiceOutput
    {
        public string Id { get; set; } = string.Empty;

        public MetadataOutput Metadata { get; set; } = new MetadataOutput();
    }
}