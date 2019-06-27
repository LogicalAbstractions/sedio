using Sedio.Core.Data;

namespace Sedio.Ignite
{
    public abstract class IgniteBranchNode : INode
    {
        public NodeId  Id { get; set; } = NodeId.None;

        public NodeMetadata Metadata { get; set; } = new NodeMetadata();

        public string? SourceId { get; set; }
    }
}