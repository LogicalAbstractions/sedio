using System;
using System.Collections.Generic;
using System.Linq;
using Sedio.Core.Collections.Paging;

namespace Sedio.Execution
{
    public interface IExecutionRequest
    {
    }

    public interface IExecutionRequest<TResponse> : IExecutionRequest
        where TResponse : IExecutionResponse
    {
    }

    public interface IPagingExecutionRequest
    {
        PagingParameters PagingParameters { get; set; }
    }

    public interface IPagingExecutionRequest<TResponse, TItem> : IExecutionRequest<TResponse>, IPagingExecutionRequest
        where TResponse : IPagingExecutionResponse<TItem>
    {

    }
}