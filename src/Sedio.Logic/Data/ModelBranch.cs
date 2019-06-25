using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Sedio.Core.Data;
using Sedio.Core.DependencyInjection;
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

    public sealed class ModelBranch : IgniteBranch<ModelBranchSchema>
    {
        internal ModelBranch(string id, 
            IIgnite ignite, 
            ModelBranchSchema schema) 
            : base(id, ignite, schema)
        {
            Services = Resolve(schema.Services);
        }

        public ICache<NodeId,ServiceNode> Services { get; }
    }

    public sealed class ModelBranchProvider : IgniteBranchProvider<ModelBranchSchema, ModelBranch>
    {
        public ModelBranchProvider(IIgnite ignite) 
            : base(new ModelBranchSchema(), ignite)
        {
        }

        protected override ModelBranch CreateInstance(IIgnite ignite, ModelBranchSchema schema, string branchId)
        {
            return new ModelBranch(branchId,ignite,schema);
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

        public static ModelBranch Branch(this ExecutionContext executionContext)
        {
            var branchId = executionContext.BranchId();
            return executionContext.BranchProvider().Get(branchId);
        }
    }
}