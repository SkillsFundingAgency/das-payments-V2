using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.Audit.Application.ServiceFabric.PaymentsEventModelCache
{
    public class PaymentsEventModelCache<T>: BatchedDataCache<T>, IPaymentsEventModelCache<T> where T: PaymentsEventModel
    {
        public PaymentsEventModelCache(
            IReliableStateManagerProvider reliableStateManagerProvider,
            IReliableStateManagerTransactionProvider transactionProvider,
            IPaymentLogger logger)
            : base(
                transactionProvider,
                reliableStateManagerProvider,
                logger
            )
        {
        }

        protected override string Describe(T message)
        {
            return message.EventId.ToString();
        }
    }
}