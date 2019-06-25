using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Sedio.Core.Collections.Paging
{
    public abstract class PagingResult
    {
        protected PagingResult(int offset, int totalCount)
        {
            Offset = offset;
            TotalCount = totalCount;
        }

        public int Offset { get; }

        public int TotalCount { get; }

        public static PagingResult<T> Empty<T>()
        {
            return new PagingResult<T>(new T[0],0,0 );
        }

        public static PagingResult<T> Create<T>(IReadOnlyList<T> items, int offset, int totalCount)
        {
            return new PagingResult<T>(items,offset,totalCount);
        }
    }

    public class PagingResult<T> : PagingResult
    {
        public PagingResult(IReadOnlyList<T> items, int offset, int totalCount)
            : base(offset,totalCount)
        {
            Items = items;
        }

        public IReadOnlyList<T> Items { get; }

        public PagingResult<TMapped> Map<TMapped>(Func<T, TMapped> mappingFunc)
        {
            return new PagingResult<TMapped>(Items.Select(mappingFunc).ToList(),Offset,TotalCount);
        }
    }
}