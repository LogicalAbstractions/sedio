using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sedio.Core.Data
{
    public sealed class NodeId : ISerializable, IEquatable<NodeId>
    {
        public static readonly NodeId None = new NodeId(typeof(void),string.Empty);

        private readonly Lazy<string[]> pathSegments;

        public Type Type { get; }
        public string Path { get; }

        public string[] PathSegments => pathSegments.Value;

        public NodeId(Type type, string path)
        {
            Type = type;
            Path = path;
            pathSegments = CreatePathSegments();
        }

        // ReSharper disable once UnusedMember.Local
#pragma warning disable 628
        protected NodeId(SerializationInfo info, StreamingContext context)
#pragma warning restore 628
        {
            Type = Type.GetType(info.GetString("type"));
            Path = info.GetString("path");
            pathSegments = CreatePathSegments();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", Type.FullName);
            info.AddValue("path",Path);
        }

        public bool Equals(NodeId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && string.Equals(Path, other.Path);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is NodeId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode() * 397) ^ Path.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{Type.FullName}:{Path}";
        }

        public string Encode()
        {
            var bytes = Encoding.UTF8.GetBytes(ToString());
            return Convert.ToBase64String(bytes);
        }

        public static NodeId Create<T>(NodeId parentId, string id)
            where T : INode
        {
            return Create<T>(parentId.Path, id);
        }

        public static NodeId Create<T>(params string[] pathSegments)
            where T : INode
        {
            return new NodeId(typeof(T),string.Join('/',pathSegments));
        }

        public static bool TryDecode(string value, out NodeId nodeId)
        {
            try
            {
                var bytes = Convert.FromBase64String(value);
                var fullString = Encoding.UTF8.GetString(bytes);

                var parts = fullString.Split(":");

                nodeId = new NodeId(System.Type.GetType(parts[0]),parts[1]);
                return true;
            }
            catch (Exception)
            {
                nodeId = None;
                return false;
            }
        }

        private Lazy<string[]> CreatePathSegments()
        {
            return new Lazy<string[]>(() => Path.Split("/"));
        }
    }
}