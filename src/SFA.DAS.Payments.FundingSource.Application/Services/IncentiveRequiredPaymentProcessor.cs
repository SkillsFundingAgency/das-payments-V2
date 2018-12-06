using System;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class IncentiveRequiredPaymentProcessor: IIncentiveRequiredPaymentProcessor
    {
        private readonly ISfaFullyFundedPaymentProcessor sfaFullyFundedPaymentProcessor;
        private readonly ISfaFullyFundedFundingSourcePaymentEventMapper mapper;

        public IncentiveRequiredPaymentProcessor(ISfaFullyFundedPaymentProcessor sfaFullyFundedPaymentProcessor, ISfaFullyFundedFundingSourcePaymentEventMapper mapper)
        {
            
            this.sfaFullyFundedPaymentProcessor = sfaFullyFundedPaymentProcessor ?? throw new ArgumentNullException(nameof(sfaFullyFundedPaymentProcessor));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public SfaFullyFundedFundingSourcePaymentEvent Process(IncentiveRequiredPaymentEvent requiredPayment)
        {
            var paymentAmount = sfaFullyFundedPaymentProcessor.CalculatePaymentAmount(requiredPayment.AmountDue);
            return mapper.Map(requiredPayment, paymentAmount);
        }
    }
}