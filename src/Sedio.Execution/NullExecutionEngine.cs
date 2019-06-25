using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sedio.Execution
{
    public sealed class NullExecutionEngine : IExecutionEngine
    {
        public static readonly NullExecutionEngine Default = new NullExecutionEngine();

#pragma warning disable 1998
        public async Task<ExecutionResult> Execute(IExecutionRequest request, 
#pragma warning restore 1998
            IReadOnlyDictionary<string, object> items, 
            CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}