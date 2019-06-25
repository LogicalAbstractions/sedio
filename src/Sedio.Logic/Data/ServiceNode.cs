using Apache.Ignite.Core.Cache.Configuration;
using Sedio.Contracts;
using Sedio.Core.Data;
using Sedio.Logic.Data.Components;

namespace Sedio.Logic.Data
{
    public sealed class ServiceNode : INode
    {
        [QuerySqlField(IsIndexed = true)]
        public NodeId Id { get; set; } = NodeId.None;
        public NodeMetadata Metadata { get; set; } = new NodeMetadata();

        [QuerySqlField(IsIndexed = true)]
        public string ServiceId { get; set; } = string.Empty;

        public ServiceOutput ToOutput() => new ServiceOutput()
        {
            Id = ServiceId,
            Metadata = Metadata.ToOutput()
        };
    }
}