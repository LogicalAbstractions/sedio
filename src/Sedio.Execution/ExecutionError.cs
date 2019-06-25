using System;

namespace Sedio.Execution
{
    public enum ExecutionErrorType
    {
        Unknown,
        NotFound,
        ValidationFailed,
        Conflict
    }

    public sealed class ExecutionError
    {
        public ExecutionError(ExecutionErrorType type,
            string? message = null,
            Exception? exception = null)
        {
            Message = message ?? exception?.Message ?? type.ToString();
            Type = type;
            Exception = exception;
        }

        public string Message { get; }

        public ExecutionErrorType Type { get; }

        public Exception? Exception { get; }
    }
}