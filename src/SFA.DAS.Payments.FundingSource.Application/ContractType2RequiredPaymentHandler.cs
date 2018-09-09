using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.FundingSource.Application
{
    public class ContractType2RequiredPaymentHandler : IContractType2RequiredPaymentHandler
    {
        private readonly IEnumerable<ICoInvestedPaymentProcessor> processors;

        public ContractType2RequiredPaymentHandler(IEnumerable<ICoInvestedPaymentProcessor> processors)
        {
            this.processors = processors ?? throw new ArgumentNullException(nameof(processors));
        }

        public IEnumerable<CoInvestedFundingSourcePaymentEvent> GetFundedPayments(ApprenticeshipContractType2RequiredPaymentEvent message)
        {
            var payments = new List<CoInvestedFundingSourcePaymentEvent>();

            foreach (var processor in processors)
            {
                var payment = processor.Process(message);

                if (payment != null && payment.AmountDue != 0)
                {
                    payments.Add(payment);
                }
            }

            return payments;
        }
    }
}