using Sedio.Contracts.Components;
using Sedio.Core.Data;

namespace Sedio.Logic.Data.Components
{
    public static class NodeMetadataExtensions
    {
        public static MetadataOutput ToOutput(this NodeMetadata metadata)
        {
            return new MetadataOutput()
            {
                CreatedAt = metadata.CreatedAt,
                UpdatedAt = metadata.UpdatedAt
            };
        }
    }
}