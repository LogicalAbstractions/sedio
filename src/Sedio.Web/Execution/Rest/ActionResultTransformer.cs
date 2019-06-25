using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sedio.Execution;

namespace Sedio.Web.Execution.Rest
{
    public sealed class ActionResultTransformer<T>
        where T : class 
    {
        private static readonly Func<T, ExecutionHostController, ActionResult>
            DefaultSuccessTransform = DefaultSuccessTransformCore;

        private static readonly Func<ExecutionErrorCollection, ExecutionHostController, ActionResult>
            DefaultErrorTransform = DefaultErrorTransformCore;

        private static ActionResult DefaultSuccessTransformCore(T value, ExecutionHostController controller)
        {
            return new OkObjectResult(value);
        }

        private static ActionResult DefaultErrorTransformCore(ExecutionErrorCollection errors, 
            ExecutionHostController controller)
        {
            var mainError = errors.FirstOrDefault(e => e.Type != ExecutionErrorType.Unknown);

            var reportingError = mainError ?? errors.First();
            var reportingErrorContent = new
            {
                reportingError.Type,
                reportingError.Message
            };

            if (mainError != null)
            {
                switch (mainError.Type)
                {
                    case ExecutionErrorType.Unknown:
                        return new ObjectResult(reportingErrorContent) { StatusCode = 500 };
                    case ExecutionErrorType.NotFound:
                        return new NotFoundObjectResult(reportingErrorContent);
                    case ExecutionErrorType.ValidationFailed:
                        return new BadRequestObjectResult(reportingErrorContent);
                    case ExecutionErrorType.Conflict:
                        return new ConflictObjectResult(reportingErrorContent);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new ObjectResult(reportingErrorContent) {StatusCode = 500};
        }

        private readonly Task<ValueOrError<T>> source;
        private readonly ExecutionHostController controller;

        internal ActionResultTransformer(Task<ValueOrError<T>> source, ExecutionHostController controller)
        {
            this.source = source;
            this.controller = controller;
        }

        public async Task<ActionResult> ToResult(
            Func<T,ExecutionHostController,ActionResult>? successTransform = null,
            Func<ExecutionErrorCollection,ExecutionHostController,ActionResult>? errorTransform = null)
        {
            var actualSuccessTransform = successTransform ?? DefaultSuccessTransform;
            var actualErrorTransform = errorTransform ?? DefaultErrorTransform;

            var valueOrError = await source.ConfigureAwait(false);

            if (valueOrError.Errors.HasErrors)
            {
                return actualErrorTransform.Invoke(valueOrError.Errors, controller);
            }

            return actualSuccessTransform.Invoke(valueOrError.Value, controller);
        }
    }
}