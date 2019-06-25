using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Transactions;

namespace Sedio.Ignite
{
    public abstract class IgniteBranch
    {
        protected IgniteBranch(
            string id, 
            IIgnite ignite, 
            IgniteBranchSchema schema)
        {
            Id = id;
            Ignite = ignite;
            Schema = schema;
        }

        public string Id { get; }
        public IIgnite Ignite { get; }
        public IgniteBranchSchema Schema { get; }
    }

    public abstract class IgniteBranch<TSchema> : IgniteBranch
        where TSchema : IgniteBranchSchema
    {
        protected IgniteBranch(string id, 
            IIgnite ignite, TSchema schema) 
            : base(id, ignite, schema)
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
    }
}