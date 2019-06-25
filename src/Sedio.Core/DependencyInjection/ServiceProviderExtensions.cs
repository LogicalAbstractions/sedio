using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sedio.Core.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static T ResolveRequired<T>(this IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService(typeof(T));

            if (service != null)
            {
                return (T) service;
            }

            throw new InvalidOperationException($"Service is not registered: {typeof(T)}");
        }

        public static IEnumerable<T> ResolveAll<T>(this IServiceProvider serviceProvider)
        {
            var resolveType = typeof(IEnumerable<>).MakeGenericType(typeof(T));

            var result = serviceProvider.GetService(resolveType) as IEnumerable;

            if (result != null)
            {
                return result.Cast<T>();
            }

            return Enumerable.Empty<T>();
        }
    }
}