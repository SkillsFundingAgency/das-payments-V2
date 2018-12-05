using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;

namespace SFA.DAS.Payments.Audit.Application.ServiceFabric.PaymentsEventProcessing
{
    public class BatchScopeFactory: IBatchScopeFactory
    {
        public IBatchScope Create()
        {
            var scope = ContainerFactory.Container.BeginLifetimeScope();
            return new BatchScope(scope);
        }
    }
}