using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{

    public class ContractType2RequiredPaymentEventFundingSourceService : IContractType2RequiredPaymentEventFundingSourceService
    {
        private readonly IEnumerable<ICoInvestedPaymentProcessor> processors;
        private readonly ICoInvestedFundingSourcePaymentEventMapper mapper;

        public ContractType2RequiredPaymentEventFundingSourceService(IEnumerable<ICoInvestedPaymentProcessor> processors, ICoInvestedFundingSourcePaymentEventMapper mapper)
        {
            this.processors = processors ?? throw new ArgumentNullException(nameof(processors));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IEnumerable<CoInvestedFundingSourcePaymentEvent> GetFundedPayments(ApprenticeshipContractType2RequiredPaymentEvent message)
        {
            var coInvestedPaymentMessage = mapper.MapToRequiredCoInvestedPayment(message);

            var paymentEvents = new List<CoInvestedFundingSourcePaymentEvent>();

            foreach (var processor in processors)
            {
                var payments = processor.Process(coInvestedPaymentMessage);
                paymentEvents.AddRange(payments.Select(payment => mapper.MapToCoInvestedPaymentEvent(message, payment)));
            }

            return paymentEvents;
        }
    }
}