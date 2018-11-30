using AutoMapper;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Handlers
{
    public class IncentivePaymentDueEventHandler :
        PaymentDueHandlerBase<IncentivePaymentDueEvent, IncentiveRequiredPaymentEvent>, IIncentivePaymentDueEventHandler
    {
        public IncentivePaymentDueEventHandler(IPaymentDueProcessor paymentDueProcessor,
            IRepositoryCache<PaymentHistoryEntity[]> paymentHistoryCache, IMapper mapper,
            IApprenticeshipKeyService apprenticeshipKeyService, IPaymentHistoryRepository paymentHistoryRepository,
            string apprenticeshipKey, IPaymentKeyService paymentKeyService)
            : base(paymentKeyService, paymentDueProcessor, paymentHistoryCache, mapper, paymentHistoryRepository,
                apprenticeshipKeyService, apprenticeshipKey)
        {
        }

        protected override IncentiveRequiredPaymentEvent CreateRequiredPayment(IncentivePaymentDueEvent paymentDue)
        {
            return new IncentiveRequiredPaymentEvent
            {
                Type = paymentDue.Type
            };
        }

        protected override int GetTransactionType(IncentivePaymentDueEvent paymentDue)
        {
            return (int) paymentDue.Type;
        }
    }
}