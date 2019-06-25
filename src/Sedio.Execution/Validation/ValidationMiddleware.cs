using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Sedio.Core.Composition;

namespace Sedio.Execution.Validation
{
    public sealed class ValidationMiddleware
    {
        public const string ExecutionContextKey = "executionContext";

        private readonly IReadOnlyList<IValidator> validators;

        public ValidationMiddleware(IEnumerable<IValidator> validators)
        {
            this.validators = validators.ToList();
        }

        public async Task Execute(ExecutionContext context, AsyncPipeline<ExecutionContext>.NextDelegate next)
        {
            var applicableValidators = validators.Where(v => v.CanValidateInstancesOfType(context.Request.GetType()));
            var validationContext = new ValidationContext(context.Request)
            {
                RootContextData = {[ExecutionContextKey] = context}
            };

            var validationMessageBuilder = new StringBuilder();
            var isInvalid = false;

            foreach (var validator in applicableValidators)
            {
                var validationResult = 
                    await validator.ValidateAsync(validationContext).ConfigureAwait(false);

                if (!validationResult.IsValid)
                {
                    validationMessageBuilder.AppendLine(validationResult.ToString());
                    isInvalid = true;
                }
            }

            if (!isInvalid)
            {
                await next.Invoke().ConfigureAwait(false);
            }
            else
            {
                context.Errors.Add(ExecutionErrorType.ValidationFailed,validationMessageBuilder.ToString());
            }
        }
    }
}