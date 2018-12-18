using AutoMapper;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Handlers
{
    public class FunctionalSkillPaymentDueEventHandler : PaymentDueHandlerBase<FunctionalSkillPaymentDueEvent, FunctionalSkillRequiredPaymentEvent>
    {
        public FunctionalSkillPaymentDueEventHandler(IPaymentKeyService paymentKeyService, IPaymentDueProcessor paymentDueProcessor, IMapper mapper) 
            : base(paymentKeyService, paymentDueProcessor, mapper)
        {
        }

        protected override FunctionalSkillRequiredPaymentEvent CreateRequiredPayment(FunctionalSkillPaymentDueEvent paymentDue)
        {
            return new FunctionalSkillRequiredPaymentEvent
            {
                Type = paymentDue.Type
            };
        }

        protected override int GetTransactionType(FunctionalSkillPaymentDueEvent paymentDue)
        {
            return (int)paymentDue.Type;
        }
    }
}