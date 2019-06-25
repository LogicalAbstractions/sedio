using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Configuration;

namespace Sedio.Ignite
{
    public abstract class IgniteCacheSchema
    {
        protected IgniteCacheSchema(
            string id, 
            Type keyType, 
            Type valueType, 
            DataRegionConfiguration region)
        {
            Id = id;
            KeyType = keyType;
            ValueType = valueType;
            Region = region;
        }

        public string Id { get; }

        public Type KeyType { get; }

        public Type ValueType { get; }

        public DataRegionConfiguration Region { get; }

        public bool Exists(IIgnite ignite,ICollection<string> cacheIds,string branchId)
        {
            return cacheIds
                .Contains(IgniteExtensions.CreateCacheId(branchId, Id));
        }

        public void Create(IIgnite ignite, string branchId)
        {
            var cacheId = IgniteExtensions.CreateCacheId(branchId, Id);
            var configuration = new CacheConfiguration(cacheId,new QueryEntity(KeyType,ValueType))
            {
                DataRegionName = Region.Name,
                AtomicityMode = CacheAtomicityMode.TransactionalSnapshot,
                CacheMode = CacheMode.Replicated,
                EnableStatistics = true
            };

            OnConfigure(configuration);

            var cache = OnCreate(ignite, configuration);
        }

        public void Delete(IIgnite ignite, string branchId)
        {
            ignite.DestroyCache(IgniteExtensions.CreateCacheId(branchId,Id));
        }

        public Task Copy(IIgnite ignite, string sourceBranchId, string targetBranchId)
        {
            return OnCopy(ignite, sourceBranchId, targetBranchId);
        }

        protected virtual void OnConfigure(CacheConfiguration configuration)
        {

        }

        protected abstract object OnCreate(IIgnite ignite,CacheConfiguration configuration);

        protected abstract Task OnCopy(IIgnite ignite, string sourceBranchId, string targetBranchId);
    }

    public class IgniteCacheSchema<TKey, TValue> : IgniteCacheSchema
    {
        private readonly Action<CacheConfiguration>? configureAction;

        public IgniteCacheSchema(
            string id, 
            DataRegionConfiguration region, 
            Action<CacheConfiguration>? configureAction) 
            : base(id, typeof(TKey),typeof(TValue), region)
        {
            this.configureAction = configureAction;
        }

        public ICache<TKey, TValue> Resolve(IIgnite ignite, string branchId)
        {
            return ignite.GetCache<TKey, TValue>(IgniteExtensions.CreateCacheId(branchId, Id));
        }

        protected override object OnCreate(IIgnite ignite,CacheConfiguration configuration)
        {
            return ignite.CreateCache<TKey, TValue>(configuration);
        }

        protected override async Task OnCopy(IIgnite ignite, string sourceBranchId, string targetBranchId)
        {
            var sourceCache = ignite.GetCache<TKey, TValue>(IgniteExtensions.CreateCacheId(sourceBranchId, Id));
            var targetCache = ignite.GetCache<TKey, TValue>(IgniteExtensions.CreateCacheId(targetBranchId, Id));

            foreach (var entry in sourceCache)
            {
                await targetCache.PutAsync(entry.Key, entry.Value).ConfigureAwait(false);
            }
        }

        protected override void OnConfigure(CacheConfiguration configuration)
        {
            configureAction?.Invoke(configuration);
        }
    }
}