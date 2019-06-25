using System;
using System.Globalization;
using Apache.Ignite.Core.Log;
using Serilog.Events;

namespace Sedio.Ignite
{
    public sealed class IgniteLogger : ILogger
    {
        private readonly Serilog.ILogger logger;
        private readonly LogLevel minLogLevel;

        public IgniteLogger(Serilog.ILogger logger,LogLevel minLogLevel)
        {
            this.logger = logger;
            this.minLogLevel = minLogLevel;
        }

        public void Log(LogLevel level, 
            string message, 
            object[] args, 
            IFormatProvider formatProvider, 
            string category,
            string nativeErrorInfo, 
            Exception ex)
        {
            if (level >= minLogLevel)
            {
                var text = message;

                if (args != null && args.Length > 0)
                {
                    text = string.Format(formatProvider ?? CultureInfo.InvariantCulture, message, args);
                }

                logger.Write(ToLevel(level),ex,text);
            }
        }

        public bool IsEnabled(LogLevel level)
        {
            return level >= minLogLevel && logger.IsEnabled(ToLevel(level));
        }

        private LogEventLevel ToLevel(LogLevel level)
        {
            return level switch {
                LogLevel.Debug => LogEventLevel.Debug,
                LogLevel.Info => LogEventLevel.Information,
                LogLevel.Warn => LogEventLevel.Warning,
                LogLevel.Error => LogEventLevel.Error,
                LogLevel.Trace => LogEventLevel.Verbose,
                _ => LogEventLevel.Verbose
                };
        }
    }
}