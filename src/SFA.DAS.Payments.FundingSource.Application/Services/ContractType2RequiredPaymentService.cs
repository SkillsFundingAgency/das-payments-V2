using AutoMapper;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class ContractType2RequiredPaymentHandler : IContractType2RequiredPaymentService
    {
        private readonly IEnumerable<ICoInvestedPaymentProcessor> processors;
        private readonly ICoInvestedFundingSourcePaymentEventMapper mapper;

        public ContractType2RequiredPaymentHandler(IEnumerable<ICoInvestedPaymentProcessor> processors, ICoInvestedFundingSourcePaymentEventMapper mapper)
        {
            this.processors = processors ?? throw new ArgumentNullException(nameof(processors));
            this.mapper = mapper;
        }

        public IEnumerable<CoInvestedFundingSourcePaymentEvent> GetFundedPayments(ApprenticeshipContractType2RequiredPaymentEvent message)
        {
            var coInvestedPaymentMessage = mapper.MapFrom(message);

            var paymentEvents = new List<CoInvestedFundingSourcePaymentEvent>();

            foreach (var processor in processors)
            {
                var payment = processor.Process(coInvestedPaymentMessage);
                if (payment != null && payment.AmountDue != 0)
                {
                    var paymentEvent = mapper.MapTo(message, payment);
                    paymentEvents.Add(paymentEvent);
                }
            }

            return paymentEvents;
        }
    }
}