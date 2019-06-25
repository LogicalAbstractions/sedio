using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
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
            var firstError = errors.First();

            var problemDetails = new ProblemDetails()
            {
                Type = "about:blank",
                Instance = controller.Request.GetEncodedUrl(),
                Status = (int)firstError.Type,
                Detail = firstError.Message,
                Title = firstError.Type.ToString()
            };

            if (firstError.Type == ExecutionErrorType.ValidationFailed)
            {
               problemDetails = new ValidationProblemDetails()
               {
                   Type = problemDetails.Type,
                   Instance = problemDetails.Instance,
                   Status = problemDetails.Status,
                   Detail = problemDetails.Detail,
                   Title = problemDetails.Title,
                   Errors = { {problemDetails.Title,
                       errors.Where(e => e.Type == ExecutionErrorType.ValidationFailed)
                           .Select(e => e.Message).ToArray()}}
               };
            }

            return new ObjectResult(problemDetails) {StatusCode = problemDetails.Status};
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