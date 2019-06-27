using NuGet.Versioning;

namespace Sedio.Contracts
{
    public class VersionInput
    {
        public NuGetVersion Version { get; set; } = new NuGetVersion(0,0,0);
    }
}