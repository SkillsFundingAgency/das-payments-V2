using System;
using Autofac;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Interfaces;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
    public class ExecutionContextFactory : IExecutionContextFactory
    {
        private readonly ILifetimeScope scope;

        public ExecutionContextFactory(ILifetimeScope scope)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IExecutionContext GetExecutionContext()
        {
            return scope.Resolve<IExecutionContext>();
        }
    }
}