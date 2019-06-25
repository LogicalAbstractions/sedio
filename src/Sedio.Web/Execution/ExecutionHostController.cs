using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Sedio.Core.Collections.Paging;
using Sedio.Execution;
using Sedio.Web.Collections;

namespace Sedio.Web.Execution
{
    public abstract class ExecutionHostController : Controller
    {
        private static readonly Func<object,ActionResult> DefaultSuccessConverter = value => new OkObjectResult(value);

        public sealed class ValueOrError<T>
            where T : class
        {
            private readonly T? value;

            internal ValueOrError(T? value, ExecutionErrorCollection errors)
            {
                this.value = value;
                Errors = errors;
            }

            public ExecutionErrorCollection Errors { get; }

            public T Value => value ?? throw new InvalidOperationException("No value present");
        }

        public sealed class SingleResponseTransformer<TResponse> 
            where TResponse : class,IExecutionResponse
        {
            private readonly Task<ExecutionResult<TResponse>> source;
            private readonly ExecutionHostController controller;

            internal SingleResponseTransformer(Task<ExecutionResult<TResponse>> source, ExecutionHostController controller)
            {
                this.source = source;
                this.controller = controller;
            }

            public ActionResultTransformer<TResponse> Pass => Transform<TResponse>(r => r);

            public ActionResultTransformer<T> Transform<T>(Func<TResponse, T> transformFunc)
                where T : class 
            {
                async Task<ValueOrError<T>> TransformWrapper()
                {
                    var response = await source.ConfigureAwait(false);

                    if (response.Errors.Any())
                    {
                        return new ValueOrError<T>(null,response.Errors);
                    }

                    return new ValueOrError<T>(transformFunc.Invoke(response.Response), response.Errors);
                }

                return new ActionResultTransformer<T>(TransformWrapper(),controller);
            }
        }

        public sealed class ListResponseTransformer<TResponse, TItem>
            where TResponse : class,IPagingExecutionResponse<TItem>
        {
            private readonly Task<ExecutionResult<TResponse>> source;
            private readonly ExecutionHostController controller;

            internal ListResponseTransformer(Task<ExecutionResult<TResponse>> source, ExecutionHostController controller)
            {
                this.source = source;
                this.controller = controller;
            }

            public ActionResultTransformer<PagingResult<TItem>> Pass => Transform<TItem>(i => i);

            public ActionResultTransformer<PagingResult<T>> Transform<T>(Func<TItem, T> transformFunc)
            
            {
                async Task<ValueOrError<PagingResult<T>>> TransformWrapper()
                {
                    var response = await source.ConfigureAwait(false);

                    if (response.Errors.Any())
                    {
                        return new ValueOrError<PagingResult<T>>(null, response.Errors);
                    }

                    return new ValueOrError<PagingResult<T>>(response.Response.Items.Map(transformFunc),response.Errors);
                }

                return new ActionResultTransformer<PagingResult<T>>(TransformWrapper(), controller);
            }
        }

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

        protected SingleResponseTransformer<TResponse> Execute<TResponse>(
            IExecutionRequest<TResponse> request)
            where TResponse : class,IExecutionResponse
        {
            return new SingleResponseTransformer<TResponse>(ExecuteTyped(request),this);
        }

        protected ListResponseTransformer<TResponse, TItem> Execute<TResponse, TItem>(
            IPagingExecutionRequest<TResponse, TItem> request)
            where TResponse : class,IPagingExecutionResponse<TItem>
        {
            return new ListResponseTransformer<TResponse, TItem>(ExecuteTyped(request),this);
        }

        private async Task<ExecutionResult<TResponse>> ExecuteTyped<TResponse>(IExecutionRequest<TResponse> request)
            where TResponse : class,IExecutionResponse
        {
            var untypedResult = await ExecuteCore(request).ConfigureAwait(false);

            return untypedResult.Cast<TResponse>();
        }

        private async Task<ExecutionResult> ExecuteCore(IExecutionRequest request)
        {
            var executionEngine = HttpContext.RequestServices.GetRequiredService<IExecutionEngine>();

            if (request is IPagingExecutionRequest listRequest)
            {
                listRequest.PagingParameters = PagingParameters.Create(
                    HttpContext.Request.Query.GetIntValueNullable("offset"),
                    HttpContext.Request.Query.GetIntValueNullable("limit")
                    );
            }

            var response = await executionEngine.Execute( 
                    request, 
                    OnGetExecutionItems(),
                    HttpContext.RequestAborted)
                .ConfigureAwait(false);

            return response;
        }

        protected abstract IReadOnlyDictionary<string, object> OnGetExecutionItems();
    }
}