using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Sedio.Web.Collections
{
    public static class StringCollectionExtensions
    {
        public static string? GetValueOrDefaultNullable(
            this IEnumerable<KeyValuePair<string, StringValues>> collection,
            string key,
            string? defaultValue)
        {
            var value = collection.FirstOrDefault(v => v.Key == key);

            if (value.Value.Count > 0)
            {
                return value.Value[0];
            }

            return defaultValue;
        }

        public static string GetValueOrDefault(
            this IEnumerable<KeyValuePair<string, StringValues>> collection,
            string key, 
            string defaultValue)
        {
            return collection.GetValueOrDefaultNullable(key, null) ?? defaultValue;
        }

        public static int? GetIntValueNullable(this IEnumerable<KeyValuePair<string, StringValues>> collection, string key)
        {
            var value = collection.GetValueOrDefaultNullable(key, null);

            if (value != null)
            {
                if (int.TryParse(value.ToString(), out var result))
                {
                    return result;
                }
            }

            return null;
        }
    }
}