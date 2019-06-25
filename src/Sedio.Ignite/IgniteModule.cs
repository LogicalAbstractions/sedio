using System.Linq;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Log;
using Autofac;
using Sedio.Core.Application;
using Sedio.Execution;
using ILogger = Serilog.ILogger;

namespace Sedio.Ignite
{
    public sealed class IgniteModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                    new IgniteLauncher(c.Resolve<IApplication>().RootPath, c.Resolve<ILogger>(), 
                        c.Resolve<IApplication>().IsProduction ? LogLevel.Warn : LogLevel.Debug))
                .AsSelf().SingleInstance();

            builder.Register(c => new IgniteProvider(c.Resolve<IgniteLauncher>()))
                .AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<IgniteProvider>().Ignite).As<IIgnite>().SingleInstance();
        }
    }
}