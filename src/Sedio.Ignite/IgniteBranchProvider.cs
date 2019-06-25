using System.Collections.Generic;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Common;
using Remotion.Linq.Clauses;

namespace Sedio.Ignite
{
    public abstract class IgniteBranchProvider<TSchema,TBranch>
        where TSchema : IgniteBranchSchema
        where TBranch : IgniteBranch<TSchema>
    {
        protected IgniteBranchProvider(TSchema schema, IIgnite ignite)
        {
            Schema = schema;
            Ignite = ignite;
        }

        public TSchema Schema { get; }

        public IIgnite Ignite { get; }

        public IReadOnlyList<string> BranchIds => Schema.GetBranchIds(Ignite);

        public TBranch Get(string branchId)
        {
            if (Schema.Exists(Ignite, branchId))
            {
                return CreateInstance(Ignite, Schema, branchId);
            }

            throw new IgniteException($"Branch {branchId} does not exist");
        }

        public bool Exists(string branchId)
        {
            return Schema.Exists(Ignite, branchId);
        }

        public void Create(string branchId)
        {
            Schema.Create(Ignite,branchId);
        }

        public void Delete(string branchId)
        {
            Schema.Delete(Ignite,branchId);
        }

        public Task Copy(string sourceBranchId, string targetBranchId)
        {
            return Schema.Copy(Ignite, sourceBranchId, targetBranchId);
        }

        protected abstract TBranch CreateInstance(IIgnite ignite, TSchema schema, string branchId);
    }
}