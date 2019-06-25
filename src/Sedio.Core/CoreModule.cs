using Autofac;
using Sedio.Core.Timing;

namespace Sedio.Core
{
    public sealed class CoreModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SystemTimeProvider>().As<ITimeProvider>().SingleInstance();
        }
    }
}