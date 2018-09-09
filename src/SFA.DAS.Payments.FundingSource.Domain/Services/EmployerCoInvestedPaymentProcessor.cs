using AutoMapper;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class EmployerCoInvestedPaymentProcessor : CoInvestedPaymentProcessor
    {
        private readonly IMapper mapper;

        public EmployerCoInvestedPaymentProcessor(IValidateRequiredPaymentEvent validateRequiredPaymentEvent, IMapper mapper)
            : base(validateRequiredPaymentEvent)
        {
            this.mapper = mapper;
        }

        protected override CoInvestedFundingSourcePaymentEvent CreatePayment(ApprenticeshipContractType2RequiredPaymentEvent message)
        {

            var amountToPay = (1 - message.SfaContributionPercentage) * message.AmountDue;
            var payment = mapper.Map<CoInvestedEmployerFundingSourcePaymentEvent>(message);
            payment.AmountDue = amountToPay;

            return payment;
        }
    }
}