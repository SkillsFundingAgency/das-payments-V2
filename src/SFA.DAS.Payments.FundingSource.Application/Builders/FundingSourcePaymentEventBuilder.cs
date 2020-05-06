using System;
using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Builders
{
    public class FundingSourcePaymentEventBuilder : IFundingSourcePaymentEventBuilder
    {
        private readonly IMapper mapper;
        private readonly IPaymentProcessor processor;

        public FundingSourcePaymentEventBuilder(IMapper mapper, IPaymentProcessor processor)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public List<FundingSourcePaymentEvent> BuildFundingSourcePaymentsForRequiredPayment(CalculatedRequiredLevyAmount requiredPaymentEvent,
            long employerAccountId, long jobId)
        {
            var fundingSourceEvents = new List<FundingSourcePaymentEvent>();
            var requiredPayment = new RequiredPayment
            {
                SfaContributionPercentage = requiredPaymentEvent.SfaContributionPercentage,
                AmountDue = requiredPaymentEvent.AmountDue,
                IsTransfer = employerAccountId != requiredPaymentEvent.AccountId
                             && requiredPaymentEvent.TransferSenderAccountId.HasValue
                             && requiredPaymentEvent.TransferSenderAccountId == employerAccountId
            };

            var fundingSourcePayments = processor.Process(requiredPayment);
            foreach (var fundingSourcePayment in fundingSourcePayments)
            {
                var fundingSourceEvent = mapper.Map<FundingSourcePaymentEvent>(fundingSourcePayment);
                mapper.Map(requiredPaymentEvent, fundingSourceEvent);
                fundingSourceEvent.JobId = jobId;
                fundingSourceEvents.Add(fundingSourceEvent);
            }

            return fundingSourceEvents;
        }
    }
}