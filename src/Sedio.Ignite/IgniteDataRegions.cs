using System.Collections.Generic;
using Apache.Ignite.Core.Configuration;

namespace Sedio.Ignite
{
    public static class IgniteDataRegions
    {
        public static readonly DataRegionConfiguration System = CreateSystem();
        public static readonly DataRegionConfiguration Persistent = CreatePersistent();
        public static readonly DataRegionConfiguration Volatile = CreateVolatile();

        public static readonly IEnumerable<DataRegionConfiguration> All = new[] {System, Persistent, Volatile};

        private static DataRegionConfiguration CreateSystem()
        {
            return new DataRegionConfiguration()
            {
                Name = "system",
                PersistenceEnabled = true
            };
        }

        private static DataRegionConfiguration CreatePersistent()
        {
            return new DataRegionConfiguration()
            {
                Name = "persistent",
                PersistenceEnabled = true
            };
        }

        private static DataRegionConfiguration CreateVolatile()
        {
            return new DataRegionConfiguration()
            {
                Name = "volatile",
                PersistenceEnabled = true
            };
        }
    }
}