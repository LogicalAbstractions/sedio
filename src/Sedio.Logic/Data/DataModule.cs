using Autofac;

namespace Sedio.Logic.Data
{
    public sealed class DataModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ModelBranchProvider>().AsSelf().SingleInstance();
            builder.Register(c => c.Resolve<ModelBranchProvider>().Schema).AsSelf().ExternallyOwned();
        }
    }
}