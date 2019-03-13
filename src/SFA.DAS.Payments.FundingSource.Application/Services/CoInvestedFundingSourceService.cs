using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Collections.Generic;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{

    public class CoInvestedFundingSourceService : ICoInvestedFundingSourceService
    {
        private readonly IEnumerable<ICoInvestedPaymentProcessorOld> processors;
        private readonly ICoInvestedFundingSourcePaymentEventMapper mapper;

        public CoInvestedFundingSourceService(IEnumerable<ICoInvestedPaymentProcessorOld> processors, ICoInvestedFundingSourcePaymentEventMapper mapper)
        {
            this.processors = processors ?? throw new ArgumentNullException(nameof(processors));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IEnumerable<CoInvestedFundingSourcePaymentEvent> GetFundedPayments(CalculatedRequiredCoInvestedAmount message)
        {
            var coInvestedPaymentMessage = mapper.MapToRequiredCoInvestedPayment(message);

            var paymentEvents = new List<CoInvestedFundingSourcePaymentEvent>();

            foreach (var processor in processors)
            {
                var payment = processor.Process(coInvestedPaymentMessage);
                if (payment != null && payment.AmountDue != 0)
                    paymentEvents.Add(mapper.MapToCoInvestedPaymentEvent(message, payment));
            }

            return paymentEvents;
        }
    }
}