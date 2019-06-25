using Apache.Ignite.Core.Cache.Configuration;
using NuGet.Versioning;
using Sedio.Core.Data;

namespace Sedio.Logic.Data
{
    public sealed class VersionNode : INode
    {
        [QuerySqlField(IsIndexed = true)]
        public NodeId Id { get; set; } = NodeId.None;

        public NodeMetadata Metadata { get; set; } = new NodeMetadata();

        // Stored as a string, since ignite.net does not support non default constructors.
        [QuerySqlField(IsIndexed = true)]
        public string Version { get; set; } = "0.0.0";
    }
}