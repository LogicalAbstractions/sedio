using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Sedio.Core.Collections.Paging;
using Sedio.Execution;

namespace Sedio.Ignite
{
    public static class IgniteExtensions
    {
        public static async Task<TValue> GetAsyncOrDefault<TKey, TValue>(this ICache<TKey, TValue> cache, TKey key,
            TValue defaultValue = default)
        {
            try
            {
                return await cache.GetAsync(key).ConfigureAwait(false);
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }
        }

        public static IgniteTransaction Transaction(this IIgnite ignite)
        {
            return new IgniteTransaction(ignite.GetTransactions().TxStart());
        }

        public static IgniteTransaction Transaction(this IgniteBranch branch)
        {
            return branch.Ignite.Transaction();
        }

        public static (string branchId, string cacheId) ParseCacheId(string fullCacheId)
        {
            var parts = fullCacheId.Split('-');

            return (parts[0], parts[1]);
        }

        public static string CreateCacheId(string branchId, string cacheId)
        {
            return $"{branchId}-{cacheId}";
        }

        public static PagingResult<TItem> ApplyPaging<TKey,TItem>( 
            this IQueryable<ICacheEntry<TKey, TItem>> queryable,
            PagingParameters pagingParameters)
        {
            var totalCount = queryable.Count();

            var items = queryable.Skip(pagingParameters.Offset).Take(pagingParameters.Limit).Select(c => c.Value);

            return PagingResult.Create(items.ToList(), pagingParameters.Offset, totalCount);
        }
    }
}