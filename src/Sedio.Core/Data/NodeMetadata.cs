using System;
using Sedio.Core.Timing;

namespace Sedio.Core.Data
{
    public sealed class NodeMetadata
    {
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public void Update(ITimeProvider timeProvider)
        {
            UpdatedAt = timeProvider.UtcNow;
        }

        public static NodeMetadata Create(ITimeProvider timeProvider)
        {
            var timeStamp = timeProvider.UtcNow;

            return new NodeMetadata()
            {
                CreatedAt = timeStamp,
                UpdatedAt = timeStamp
            };
        }
    }
}