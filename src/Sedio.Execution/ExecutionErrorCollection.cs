using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Sedio.Execution
{
    public sealed class ExecutionErrorCollection : IEnumerable<ExecutionError>
    {
        private readonly List<ExecutionError> errors = new List<ExecutionError>();

        public bool HasErrors => errors.Any();

        public void Add(ExecutionError error)
        {
            this.errors.Add(error);
        }

        public void Add(IEnumerable<ExecutionError> errors)
        {
            this.errors.AddRange(errors);
        }

        public void Add(ExecutionErrorType type, string? message = null, Exception? exception = null)
        {
            Add(new ExecutionError(type,message,exception));
        }

        public IEnumerator<ExecutionError> GetEnumerator()
        {
            return errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}