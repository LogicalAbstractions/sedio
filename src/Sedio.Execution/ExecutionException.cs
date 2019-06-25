using System;

namespace Sedio.Execution
{
    public class ExecutionException : Exception
    {
        public ExecutionException(ExecutionErrorType type, 
            string? message = null,
            Exception? innerException = null)
            : base(message ?? innerException?.Message ?? type.ToString(),
                innerException)
        {
            Type = type;
        }

        public ExecutionErrorType Type { get; }

        public ExecutionError ToError()
        {
            return new ExecutionError(Type,Message,InnerException);
        }
    }
}