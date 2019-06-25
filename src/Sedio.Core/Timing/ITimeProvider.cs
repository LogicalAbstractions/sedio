using System;

namespace Sedio.Core.Timing
{
    public interface ITimeProvider
    {
        DateTimeOffset UtcNow { get; }
    }
}