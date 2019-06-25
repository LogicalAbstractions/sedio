using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Configuration;
using Apache.Ignite.Core.Impl.Collections;

namespace Sedio.Ignite
{
    public abstract class IgniteBranchSchema
    {
        private readonly Dictionary<string,IgniteCacheSchema> cacheSchemata = 
            new Dictionary<string, IgniteCacheSchema>();

        protected IgniteCacheSchema<TKey, TValue> Declare<TKey, TValue>(
            string id,
            DataRegionConfiguration region,
            Action<CacheConfiguration>? configureAction = null
            )
        {
            var schema = new IgniteCacheSchema<TKey,TValue>(id,region,configureAction);
            cacheSchemata[id] = schema;

            return schema;
        }

        public IReadOnlyList<string> GetBranchIds(IIgnite ignite)
        {
            var cacheNames = ignite.GetCacheNames();
            var potentialBranchIds = cacheNames.Select(IgniteExtensions.ParseCacheId)
                .GroupBy(c => c.branchId)
                .Where(c => cacheSchemata.Values.All(v => v.Exists(ignite, cacheNames, c.Key)))
                .Select(c => c.Key)
                .ToList();

            return potentialBranchIds;
        }

        public bool Exists(IIgnite ignite, string branchId)
        {
            var cacheNames = ignite.GetCacheNames();

            return cacheSchemata.Values.All(v => v.Exists(ignite, cacheNames, branchId));
        }

        public void Create(IIgnite ignite, string branchId)
        {
            var createdCaches = new List<IgniteCacheSchema>();

            var cacheIds = ignite.GetCacheNames();
         
            foreach (var cacheSchema in cacheSchemata.Values)
            {
                try
                {
                    if (!cacheSchema.Exists(ignite, cacheIds, branchId))
                    {
                        cacheSchema.Create(ignite, branchId);
                        createdCaches.Add(cacheSchema);
                    }
                }
                catch (Exception)
                {
                    foreach (var createdCache in createdCaches)
                    {
                        createdCache.Delete(ignite,branchId);
                    }
                    throw;
                }
            }
        }

        public void Delete(IIgnite ignite, string branchId)
        {
            foreach (var cache in cacheSchemata.Values)
            {
                cache.Delete(ignite,branchId);
            }
        }

        public async Task Copy(IIgnite ignite, string sourceBranchId, string targetBranchId)
        {
            foreach (var cache in cacheSchemata.Values)
            {
                await cache.Copy(ignite, sourceBranchId, targetBranchId).ConfigureAwait(false);
            }
        }
    }
}