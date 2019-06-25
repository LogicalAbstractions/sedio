using System;
using System.Threading;
using Apache.Ignite.Core;

namespace Sedio.Ignite
{
    public sealed class IgniteProvider : IDisposable
    {
        private readonly Lazy<IIgnite> instance;

        public IgniteProvider(IgniteLauncher launcher)
        {
            this.instance = new Lazy<IIgnite>(launcher.Launch,LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public IIgnite Ignite => instance.Value;

        public void Dispose()
        {
            if (instance.IsValueCreated)
            {
                instance.Value.Dispose();
            }
        }
    }
}