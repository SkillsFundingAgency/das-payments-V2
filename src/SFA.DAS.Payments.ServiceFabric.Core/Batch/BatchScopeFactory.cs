using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;

namespace SFA.DAS.Payments.ServiceFabric.Core.Batch
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