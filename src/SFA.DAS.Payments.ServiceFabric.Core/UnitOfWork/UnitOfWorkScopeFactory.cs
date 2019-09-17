using System;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;

namespace SFA.DAS.Payments.ServiceFabric.Core.UnitOfWork
{
    public class UnitOfWorkScopeFactory : IUnitOfWorkScopeFactory
    {
        public IUnitOfWorkScope Create(string operationName)
        {
            var scope = ContainerFactory.Container.BeginLifetimeScope();
            return new UnitOfWorkScope(scope, operationName ?? $"PaymentProcessing:{Guid.NewGuid():N}");
        }
    }
}