using System;
using Autofac;
using ESFA.DC.Logging;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
    public class ExecutionContextFactory : IExecutionContextFactory
    {
        private readonly ILifetimeScope scope;

        public ExecutionContextFactory(ILifetimeScope scope)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public ExecutionContext GetExecutionContext()
        {
            return scope.Resolve<ExecutionContext>();
        }
    }
}