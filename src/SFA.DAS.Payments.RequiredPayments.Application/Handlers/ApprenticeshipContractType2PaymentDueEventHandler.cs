using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Handlers
{
    public class ApprenticeshipContractType2PaymentDueEventHandler : PaymentDueHandlerBase<ApprenticeshipContractType2PaymentDueEvent, ApprenticeshipContractType2RequiredPaymentEvent>, IApprenticeshipContractType2PaymentDueEventHandler
    {
        public ApprenticeshipContractType2PaymentDueEventHandler(IPaymentDueProcessor paymentDueProcessor, IRepositoryCache<PaymentHistoryEntity[]> paymentHistoryCache, IMapper mapper, 
            IApprenticeshipKeyService apprenticeshipKeyService, IPaymentHistoryRepository paymentHistoryRepository, string apprenticeshipKey, IPaymentKeyService paymentKeyService)
        : base(paymentKeyService, paymentDueProcessor, paymentHistoryCache, mapper, paymentHistoryRepository, apprenticeshipKeyService, apprenticeshipKey)
        {
        }

        protected override ApprenticeshipContractType2RequiredPaymentEvent CreateRequiredPayment(ApprenticeshipContractType2PaymentDueEvent paymentDue)
        {
            return new ApprenticeshipContractType2RequiredPaymentEvent
            {
                OnProgrammeEarningType = paymentDue.Type,
                SfaContributionPercentage = paymentDue.SfaContributionPercentage,
            };
        }

        protected override int GetTransactionType(ApprenticeshipContractType2PaymentDueEvent paymentDue)
        {
            return (int)paymentDue.Type;
        }
    }
}
