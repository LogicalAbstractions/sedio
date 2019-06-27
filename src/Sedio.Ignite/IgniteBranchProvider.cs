using System.Collections.Generic;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Common;
using Remotion.Linq.Clauses;
using Sedio.Core.Data;
using Sedio.Core.Timing;

namespace Sedio.Ignite
{
    public abstract class IgniteBranchProvider<TSchema,TBranch,TBranchNode>
        where TSchema : IgniteBranchSchema
        where TBranch : IgniteBranch<TSchema,TBranchNode>
        where TBranchNode : IgniteBranchNode
    {
        private readonly ICache<NodeId, TBranchNode> branchCache;
        private readonly ITimeProvider timeProvider;

        protected IgniteBranchProvider(TSchema schema, IIgnite ignite, ITimeProvider timeProvider)
        {
            Schema = schema;
            Ignite = ignite;
            this.timeProvider = timeProvider;

            this.branchCache = ignite.GetOrCreateCache<NodeId, TBranchNode>(
                new CacheConfiguration("branches", new QueryEntity(typeof(NodeId), typeof(TBranchNode)))
                {
                    DataRegionName = IgniteDataRegions.System.Name,
                    AtomicityMode = CacheAtomicityMode.TransactionalSnapshot,
                    CacheMode = CacheMode.Replicated,
                    EnableStatistics = true
                });
        }

        public TSchema Schema { get; }

        public IIgnite Ignite { get; }

        public IReadOnlyList<string> BranchIds => Schema.GetBranchIds(Ignite);

        public async Task<TBranch> Get(string branchId)
        {
            var node = await branchCache.GetAsyncOrDefault(NodeId.Create<TBranchNode>(branchId))
                .ConfigureAwait(false);

            if (node != null)
            {
                return CreateInstance(Ignite, Schema, node);
            }

            throw new IgniteException($"Branch {branchId} does not exist");
        }

        public Task<bool> Exists(string branchId)
        {
            return branchCache.ContainsKeyAsync(NodeId.Create<TBranchNode>(branchId));
        }

        public async Task Create(string branchId,string? sourceBranchId)
        {
            var node = CreateNode();

            node.Id = NodeId.Create<TBranchNode>(branchId);
            node.Metadata = NodeMetadata.Create(timeProvider);
            node.SourceId = sourceBranchId;

            if (await branchCache.PutIfAbsentAsync(node.Id, node).ConfigureAwait(false))
            {
                Schema.Create(Ignite, branchId);

                if (sourceBranchId != null)
                {
                    if (await Exists(sourceBranchId).ConfigureAwait(false))
                    {
                        await Schema.Copy(Ignite, sourceBranchId, branchId).ConfigureAwait(false);
                    }
                    else
                    {
                        throw new IgniteException($"Branch {sourceBranchId} does not exist");
                    }
                }
            }
        }

        public async Task Delete(string branchId)
        {
            if (await branchCache.RemoveAsync(NodeId.Create<TBranchNode>(branchId))
                .ConfigureAwait(false))
            {
                Schema.Delete(Ignite, branchId);
            }
        }

        protected abstract TBranchNode CreateNode();

        protected abstract TBranch CreateInstance(IIgnite ignite, TSchema schema, TBranchNode node);
    }
}