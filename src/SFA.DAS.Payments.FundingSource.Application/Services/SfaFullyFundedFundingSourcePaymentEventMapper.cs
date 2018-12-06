using System;
using AutoMapper;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class SfaFullyFundedFundingSourcePaymentEventMapper : ISfaFullyFundedFundingSourcePaymentEventMapper
    {
        private readonly IMapper mapper;

        public SfaFullyFundedFundingSourcePaymentEventMapper(IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public SfaFullyFundedFundingSourcePaymentEvent Map(IncentiveRequiredPaymentEvent requiredPaymentsEvent,
            FundingSourcePayment payment)
        {
            var sfaFullyFundedPayment = mapper.Map<SfaFullyFundedFundingSourcePaymentEvent>(requiredPaymentsEvent);
            sfaFullyFundedPayment.AmountDue = payment.AmountDue;
            return sfaFullyFundedPayment;
        }
    }
}