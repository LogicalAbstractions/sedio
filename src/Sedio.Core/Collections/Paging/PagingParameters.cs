using System.Collections.Generic;
using System.Linq;

namespace Sedio.Core.Collections.Paging
{
    public sealed class PagingParameters
    {
        public const int DefaultLimit = 8;
        public const int MaxLimit = 128;

        private PagingParameters(int offset, int limit)
        {
            Offset = offset;
            Limit = limit;
        }

        public int Offset { get; }

        public int Limit { get; }

        public static PagingParameters Create(int? offset = null, int? limit = null)
        {
            var actualOffset = offset ?? 0;
            var coercedOffset = actualOffset < 0 ? 0 : actualOffset;

            var actualLimit = limit ?? DefaultLimit;

            var coercedLimit = actualLimit < 1 ? 1 : actualLimit;
            coercedLimit = coercedLimit > MaxLimit ? MaxLimit: coercedLimit;

            return new PagingParameters(coercedOffset,coercedLimit);
        }
    }

    public static class PagingParametersExtensions
    {
        public static PagingResult<T> ApplyPaging<T>(this IEnumerable<T> input, PagingParameters pagingParameters)
        {
            var totalCount = input.Count();

            return PagingResult.Create(input.Skip(pagingParameters.Offset).Take(pagingParameters.Limit).ToList(),
                pagingParameters.Offset, totalCount);
        }
    }
}