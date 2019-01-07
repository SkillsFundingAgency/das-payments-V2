using AutoMapper;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Handlers
{
    public class IncentivePaymentDueEventHandler : PaymentDueHandlerBase<IncentivePaymentDueEvent, IncentiveRequiredPaymentEvent>
    {
        public IncentivePaymentDueEventHandler(IPaymentDueProcessor paymentDueProcessor, IMapper mapper, IPaymentKeyService paymentKeyService)
            : base(paymentKeyService, paymentDueProcessor, mapper)
        {
        }

        protected override IncentiveRequiredPaymentEvent CreateRequiredPayment(IncentivePaymentDueEvent paymentDue)
        {
            return new IncentiveRequiredPaymentEvent
            {
                Type = paymentDue.Type,
                ContractType = paymentDue.ContractType,
                PaymentsDueEventId = paymentDue.EventId
            };
        }

        protected override int GetTransactionType(IncentivePaymentDueEvent paymentDue)
        {
            return (int)paymentDue.Type;
        }
    }
}