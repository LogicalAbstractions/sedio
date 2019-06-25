using System;
using Autofac;
using FluentValidation;
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

            builder.RegisterAssemblyTypes(GetType().Assembly)
                .Where(t => t.IsAssignableTo<IValidator>() && !t.IsAbstract && t.IsClass)
                .As<IValidator>()
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