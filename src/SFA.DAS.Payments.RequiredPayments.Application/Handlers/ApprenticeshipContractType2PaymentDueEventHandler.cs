using AutoMapper;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Handlers
{
    public class ApprenticeshipContractType2PaymentDueEventHandler : PaymentDueHandlerBase<ApprenticeshipContractType2PaymentDueEvent, ApprenticeshipContractType2RequiredPaymentEvent>
    {
        public ApprenticeshipContractType2PaymentDueEventHandler(IPaymentDueProcessor paymentDueProcessor, IMapper mapper, IPaymentKeyService paymentKeyService)
            : base(paymentKeyService, paymentDueProcessor, mapper)
        {
        }

        protected override ApprenticeshipContractType2RequiredPaymentEvent CreateRequiredPayment(ApprenticeshipContractType2PaymentDueEvent paymentDue)
        {
            return new ApprenticeshipContractType2RequiredPaymentEvent
            {
                OnProgrammeEarningType = paymentDue.Type,
                SfaContributionPercentage = paymentDue.SfaContributionPercentage,
                PaymentsDueEventId = paymentDue.EventId
            };
        }

        protected override int GetTransactionType(ApprenticeshipContractType2PaymentDueEvent paymentDue)
        {
            return (int) paymentDue.Type;
        }
    }
}