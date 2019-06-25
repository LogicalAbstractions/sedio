using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Configuration;
using Apache.Ignite.Core.Lifecycle;
using Apache.Ignite.Core.Log;
using ILogger = Serilog.ILogger;

namespace Sedio.Ignite
{
    public sealed class IgniteLauncher
    {
        private readonly string rootPath;
        private readonly Serilog.ILogger logger;
        private readonly LogLevel minLogLevel;

        public IgniteLauncher(string rootPath, ILogger logger, LogLevel minLogLevel)
        {
            this.rootPath = rootPath;
            this.logger = logger;
            this.minLogLevel = minLogLevel;
        }

        public IIgnite Launch()
        {
            var jvmDllPath = GetJvmDllPath();

            if (!File.Exists(jvmDllPath))
            {
                throw new FileNotFoundException("Jvm not found", jvmDllPath);
            }

            var configuration = new IgniteConfiguration()
            {
                ClientMode = false,
                JvmInitialMemoryMb = 512,
                JvmMaxMemoryMb = 8192,
                MetricsLogFrequency = TimeSpan.Zero,
                JvmDllPath = jvmDllPath,
                JvmClasspath = GetJvmClassPath(),
                JvmOptions = new List<string>()
                {
                    
                },
                Logger = new IgniteLogger(logger, minLogLevel),
                LifecycleHandlers = new List<ILifecycleHandler>()
                {

                },
                DataStorageConfiguration = new DataStorageConfiguration()
                {
                    DataRegionConfigurations = new List<DataRegionConfiguration>(IgniteDataRegions.All),
                    StoragePath = GetDirectory(true,"Data", "Storage")
                },
                WorkDirectory = GetDirectory(true, "Data", "Work")
            };

            var result = Ignition.Start(configuration);
            result.GetCluster().SetActive(true);

            return result;
        }

        private string GetDirectory(bool createIfNotFound,params string[] pathParts)
        {
            var relativePath = Path.Combine(pathParts);
            var fullPath = Path.Combine(rootPath, relativePath);

            if (createIfNotFound && !Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return fullPath;
        }

        private string GetJvmClassPath()
        {
            var directory = Path.Combine(rootPath, "3rdParty", "Ignite");

            return string.Join(";", Directory.GetFiles(directory, "*.jar"));
        }

        private string GetJvmDllPath()
        {
            return Path.Combine(rootPath,
                "3rdParty",
                "Ignite",
                "jre",
                GetOsPrefix(),
                "bin",
                "server",
                GetJvmFilename());
        }

        private static string GetOsPrefix()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "win64";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "linux64";
            }

            return "unknown";
        }

        private static string GetJvmFilename()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "jvm.dll";
            }

            return "libjvm.so";
        }
    }
}