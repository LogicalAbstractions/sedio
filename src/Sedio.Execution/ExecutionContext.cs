using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Sedio.Core.DependencyInjection;
using Sedio.Core.Timing;
using Serilog;

namespace Sedio.Execution
{
    public class ExecutionContext
    {
        public ExecutionContext(
            IServiceProvider services, 
            ILogger logger,
            IExecutionRequest request,
            IReadOnlyDictionary<string,object> items,
            CancellationToken cancellationToken)
        {
            Services = services;
            Log = logger;
            Request = request;
            CancellationToken = cancellationToken;
            Items = new Dictionary<string, object>(items);
            Errors = new ExecutionErrorCollection();
        }

        public CancellationToken CancellationToken { get; }

        public IServiceProvider Services { get; }

        public ILogger Log { get; }

        public IDictionary<string, object> Items { get; }

        public ExecutionErrorCollection Errors { get; }

        public IExecutionRequest Request { get; set; }

        public IExecutionResponse? Response { get; set; }
    }

    public static class ExecutionContextExtensions
    {
        public static ITimeProvider TimeProvider(this ExecutionContext context)
        {
            return context.Services.ResolveRequired<ITimeProvider>();
        }
    }
}