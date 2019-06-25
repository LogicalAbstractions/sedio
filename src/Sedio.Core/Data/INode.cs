namespace Sedio.Core.Data
{
    public interface INode
    {
        NodeId Id { get; set; }

        NodeMetadata Metadata { get; set; }
    }
}