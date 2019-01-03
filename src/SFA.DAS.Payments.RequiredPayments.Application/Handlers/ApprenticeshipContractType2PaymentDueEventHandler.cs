using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Handlers
{
    public class ApprenticeshipContractType2PaymentDueEventHandler : PaymentDueHandlerBase<ApprenticeshipContractType2PaymentDueEvent, ApprenticeshipContractType2RequiredPaymentEvent>
    {
        public ApprenticeshipContractType2PaymentDueEventHandler(IPaymentDueProcessor paymentDueProcessor, IMapper mapper, IPaymentKeyService paymentKeyService)
            : base(paymentKeyService, paymentDueProcessor, mapper)
        {
        }

        protected override ApprenticeshipContractType2RequiredPaymentEvent CreateRequiredPayment(ApprenticeshipContractType2PaymentDueEvent paymentDue, Payment[] payments)
        {
            var sfaContributionPercentage = paymentDue.SfaContributionPercentage;

            // YUCK: refund with 0 earning
            // TODO: work out the better way of doing it
            if (sfaContributionPercentage == 0 && paymentDue.AmountDue == 0 && payments.Length > 0)
            {
                var sfaContribution = payments.Where(p => p.FundingSource == FundingSourceType.CoInvestedSfa).Sum(p => p.Amount);
                var employerContribution = payments.Where(p => p.FundingSource == FundingSourceType.CoInvestedEmployer).Sum(p => p.Amount);
                sfaContributionPercentage = sfaContribution / (sfaContribution + employerContribution);
            }

            return new ApprenticeshipContractType2RequiredPaymentEvent
            {
                OnProgrammeEarningType = paymentDue.Type,
                SfaContributionPercentage = sfaContributionPercentage
            };
        }

        protected override int GetTransactionType(ApprenticeshipContractType2PaymentDueEvent paymentDue)
        {
            return (int) paymentDue.Type;
        }
    }
}