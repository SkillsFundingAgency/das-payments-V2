using System;
using SFA.DAS.Payments.Audit.Application.Mapping.FundingSource;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Messaging.Serialization;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource
{
    public class FundingSourceEventMessageModifier : IApplicationMessageModifier
    {
        private readonly IFundingSourceEventMapper mapper;

        public FundingSourceEventMessageModifier(IFundingSourceEventMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public object Modify(object applicationMessage)
        {
            var fundingSourceEvent = applicationMessage as IFundingSourcePaymentEvent;
            return fundingSourceEvent == null ? applicationMessage : mapper.Map(fundingSourceEvent);
        }
    }
}