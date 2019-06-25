using System;
using System.Collections.Generic;
using Sedio.Core.Collections.Paging;

namespace Sedio.Execution
{
    public interface IExecutionResponse
    {

    }

    public interface IPagingExecutionResponse : IExecutionResponse
    {
        PagingResult Items { get; }
    }

    public interface IPagingExecutionResponse<T> : IPagingExecutionResponse
    {
        new PagingResult<T> Items { get; set; }
    }

    public sealed class PagingExecutionResponse<T> : IPagingExecutionResponse<T>
    {
        PagingResult IPagingExecutionResponse.Items => this.Items;
        public PagingResult<T> Items { get; set; } = PagingResult.Empty<T>();
    }
}