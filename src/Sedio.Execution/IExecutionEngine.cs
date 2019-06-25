using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Sedio.Execution
{
    public interface IExecutionEngine
    {
        Task<ExecutionResult> Execute(
            IExecutionRequest request, 
            IReadOnlyDictionary<string,object> items,
            CancellationToken cancellationToken);
    }
}