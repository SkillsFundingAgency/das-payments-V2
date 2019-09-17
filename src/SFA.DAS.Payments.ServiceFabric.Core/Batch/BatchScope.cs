using Autofac;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.ServiceFabric.Core.UnitOfWork;

namespace SFA.DAS.Payments.ServiceFabric.Core.Batch
{
    public class BatchScope: UnitOfWorkScope,  IBatchScope
    {
        public BatchScope(ILifetimeScope lifetimeScope): base(lifetimeScope, "AuditBatchProcessing")
        {
        }


        public IBatchProcessor<T> GetBatchProcessor<T>()
        {
            return LifetimeScope.Resolve<IBatchProcessor<T>>();
        }

    }
}