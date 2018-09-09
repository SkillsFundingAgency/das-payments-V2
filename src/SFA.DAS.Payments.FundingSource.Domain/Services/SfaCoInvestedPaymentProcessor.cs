using AutoMapper;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class SfaCoInvestedPaymentProcessor : CoInvestedPaymentProcessor
    {
        private readonly IMapper mapper;

        public SfaCoInvestedPaymentProcessor(IValidateRequiredPaymentEvent validateRequiredPaymentEvent, IMapper mapper)
            : base(validateRequiredPaymentEvent)
        {
            this.mapper = mapper;
        }

        protected override CoInvestedFundingSourcePaymentEvent CreatePayment(ApprenticeshipContractType2RequiredPaymentEvent message)
        {
            var amountToPay = message.SfaContributionPercentage * message.AmountDue;

            var payment = mapper.Map<CoInvestedSfaFundingSourcePaymentEvent>(message);
            payment.AmountDue = amountToPay;

            return payment;
        }
    }
}