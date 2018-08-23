using System;
using Autofac;
using ESFA.DC.Logging;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
    public interface IExecutionContextFactory
    {
        ExecutionContext GetExecutionContext();
    }

    public class ExecutionContextFactory : IExecutionContextFactory
    {
        private readonly ILifetimeScope _scope;

        public ExecutionContextFactory(ILifetimeScope scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public ExecutionContext GetExecutionContext()
        {
            return _scope.Resolve<ExecutionContext>();
        }
    }
}