using System.Collections.Generic;
using System.Threading.Tasks;
using Sedio.Core.Composition;

namespace Sedio.Execution
{
    internal sealed class ExecutionControllerDispatcher
    {
        private readonly IReadOnlyList<IExecutionController> controllers;

        internal ExecutionControllerDispatcher(IReadOnlyList<IExecutionController> controllers)
        {
            this.controllers = controllers;
        }

        internal async Task Dispatch(ExecutionContext context,AsyncPipeline<ExecutionContext>.NextDelegate next)
        {
            foreach (var controller in controllers)
            {
                if (controller.CanExecute(context))
                {
                    await controller.Execute(context).ConfigureAwait(false);
                    return;
                }
            }

            await next.Invoke().ConfigureAwait(false);
        }
    }
}