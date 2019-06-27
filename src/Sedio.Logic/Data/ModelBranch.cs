using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Sedio.Core.Data;
using Sedio.Core.DependencyInjection;
using Sedio.Core.Timing;
using Sedio.Execution;
using Sedio.Ignite;

namespace Sedio.Logic.Data
{
    public sealed class ModelBranchSchema : IgniteBranchSchema
    {
        public ModelBranchSchema()
        {
            Services = Declare<NodeId, ServiceNode>("services", IgniteDataRegions.Persistent);
        }

        public IgniteCacheSchema<NodeId, ServiceNode> Services { get; }
    }

    public sealed class ModelBranchNode : IgniteBranchNode
    {
    }

    public sealed class ModelBranch : IgniteBranch<ModelBranchSchema,ModelBranchNode>
    {
        internal ModelBranch(
            IIgnite ignite, 
            ModelBranchSchema schema,
            ModelBranchNode node) 
            : base(ignite, schema,node)
        {
            Services = Resolve(schema.Services);
        }

        public ICache<NodeId,ServiceNode> Services { get; }
    }

    public sealed class ModelBranchProvider : IgniteBranchProvider<ModelBranchSchema, ModelBranch,ModelBranchNode>
    {
        public ModelBranchProvider(IIgnite ignite,ITimeProvider timeProvider) 
            : base(new ModelBranchSchema(), ignite,timeProvider)
        {
        }

        protected override ModelBranchNode CreateNode()
        {
            return new ModelBranchNode();
        }

        protected override ModelBranch CreateInstance(IIgnite ignite, ModelBranchSchema schema, ModelBranchNode node)
        {
            return new ModelBranch(ignite, schema, node);
        }
    }

    public static class ModelBranchExtensions
    {
        public const string BranchIdKey = "branchId";

        public static string BranchId(this ExecutionContext executionContext)
        {
            if (executionContext.Items.TryGetValue(BranchIdKey, out var branchId))
            {
                return branchId.ToString();
            }

            throw new ExecutionException(ExecutionErrorType.Unknown,"Branch id missing");
        }

        public static ModelBranchProvider BranchProvider(this ExecutionContext executionContext)
        {
            return executionContext.Services.ResolveRequired<ModelBranchProvider>();
        }

        public static Task<ModelBranch> Branch(this ExecutionContext executionContext)
        {
            var branchId = executionContext.BranchId();
            return executionContext.BranchProvider().Get(branchId);
        }
    }
}