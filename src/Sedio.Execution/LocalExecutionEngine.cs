using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Sedio.Core.Composition;
using Sedio.Core.DependencyInjection;
using Sedio.Execution.Validation;
using Serilog;

namespace Sedio.Execution
{
    public sealed class LocalExecutionEngine : IExecutionEngine
    {
        public sealed class Builder
        {
            private readonly IServiceProvider serviceProvider;
            private readonly List<AsyncPipeline<ExecutionContext>.StepDelegate> steps;

            internal Builder(IServiceProvider serviceProvider)
            {
                this.Services = serviceProvider;
                this.steps = new List<AsyncPipeline<ExecutionContext>.StepDelegate>();
            }

            public IServiceProvider Services { get; }

            public Builder Use(AsyncPipeline<ExecutionContext>.StepDelegate step)
            {
                steps.Add(step);
                return this;
            }

            public Builder ValidateRequests()
            {
                return ValidateRequests(serviceProvider.ResolveAll<IValidator>());
            }

            public Builder ValidateRequests(IEnumerable<IValidator> validators)
            {
                return Use(new ValidationMiddleware(validators).Execute);
            }

            public Builder TransformExceptions()
            {
                return Use(async (input, next) =>
                {
                    static bool AddExceptionError(ExecutionContext context, Exception ex)
                    {
                        if (ex is ExecutionException executionException)
                        {
                            context.Errors.Add(executionException.ToError());
                        }
                        else
                        {
                            context.Errors.Add(new ExecutionError(ExecutionErrorType.Unknown,exception:ex));
                        }

                        return true;
                    }

                    try
                    {
                        await next().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        if (!AddExceptionError(input, ex))
                        {
                            throw;
                        }
                    }
                });
            }

            public Builder UseControllers()
            {
                return UseControllers(Services.ResolveAll<IExecutionController>());
            }

            public Builder UseControllers(IEnumerable<IExecutionController> controllers)
            {
                return Use(new ExecutionControllerDispatcher(controllers.ToList()).Dispatch);
            }

            public IExecutionEngine Build()
            {
                var pipeline = AsyncPipeline<ExecutionContext>.Compile(steps);

                return new LocalExecutionEngine(Services,pipeline);
            }
        }

        private readonly IServiceProvider serviceProvider;
        private readonly AsyncPipeline<ExecutionContext>.InvocationDelegate pipeline;

        internal LocalExecutionEngine(IServiceProvider serviceProvider,
            AsyncPipeline<ExecutionContext>.InvocationDelegate pipeline)
        {
            this.serviceProvider = serviceProvider;
            this.pipeline = pipeline;
        }

        public static Builder New(IServiceProvider serviceProvider)
        {
            return new Builder(serviceProvider);
        }

        public async Task<ExecutionResult> Execute(
            IExecutionRequest request,
            IReadOnlyDictionary<string,object> items, 
            CancellationToken cancellationToken)
        {
            var logger = serviceProvider.ResolveRequired<ILogger>();

            var context = new ExecutionContext(serviceProvider,logger,request,items,cancellationToken);

            await pipeline.Invoke(context).ConfigureAwait(false);

            return new ExecutionResult(context.Response, context.Errors);
        }
    }
}