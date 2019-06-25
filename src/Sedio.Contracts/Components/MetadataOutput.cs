using System;

namespace Sedio.Contracts.Components
{
    public sealed class MetadataOutput
    {
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}