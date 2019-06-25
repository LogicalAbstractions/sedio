using System;

namespace Sedio.Execution
{
    public enum ExecutionErrorType
    {
        Unknown = 500,
        NotFound = 404,
        ValidationFailed = 400,
        Conflict = 409
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