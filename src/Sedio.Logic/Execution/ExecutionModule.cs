using System;
using Autofac;
using Sedio.Execution;

namespace Sedio.Logic.Execution
{
    public sealed class ExecutionModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(GetType().Assembly)
                .Where(t => t.IsAssignableTo<IExecutionController>() && !t.IsAbstract && t.IsClass)
                .As<IExecutionController>()
                .SingleInstance();

            builder.Register(c =>
                LocalExecutionEngine.New(c.Resolve<IServiceProvider>())
                    .TransformExceptions()
                    .ValidateRequests()
                    .UseControllers()
                    .Build()
                ).As<IExecutionEngine>().SingleInstance();
        }
    }
}