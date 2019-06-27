using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Transactions;

namespace Sedio.Ignite
{
    public abstract class IgniteBranch
    {
        protected IgniteBranch(
            IIgnite ignite, 
            IgniteBranchSchema schema, 
            IgniteBranchNode node)
        {
            Id = node.Id.Path;
            Ignite = ignite;
            Schema = schema;
            Node = node;
        }

        public string Id { get; }
        public IIgnite Ignite { get; }
        public IgniteBranchSchema Schema { get; }
        public IgniteBranchNode Node { get; }
    }

    public abstract class IgniteBranch<TSchema,TBranchNode> : IgniteBranch
        where TSchema : IgniteBranchSchema
        where TBranchNode : IgniteBranchNode
    {
        protected IgniteBranch(
            IIgnite ignite, TSchema schema,TBranchNode branchNode) 
            : base(ignite, schema,branchNode)
        {
          
        }

        protected ICache<TKey, TValue> Resolve<TKey, TValue>(IgniteCacheSchema<TKey, TValue> schema)
        {
            return schema.Resolve(Ignite, Id);
        }

        public ITransaction BeginTransaction()
        {
            return Ignite.GetTransactions().TxStart();
        }

        public new TSchema Schema => (TSchema) base.Schema;
        public new TBranchNode Node => (TBranchNode) base.Node;
    }
}