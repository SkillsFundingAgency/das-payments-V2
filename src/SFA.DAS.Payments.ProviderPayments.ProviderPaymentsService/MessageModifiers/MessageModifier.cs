using System;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Mapping.FundingSource;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Messaging.Serialization;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.MessageModifiers
{
    public class MessageModifier: IApplicationMessageModifier
    {
        private readonly IPaymentLogger logger;
        private readonly IFundingSourceEventMapper mapper;

        public MessageModifier(IPaymentLogger logger, IFundingSourceEventMapper mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public object Modify(object applicationMessage)
        {
            var fundingSourceEvent = applicationMessage as FundingSourcePaymentEvent;
            return fundingSourceEvent == null ? applicationMessage : mapper.Map(fundingSourceEvent);
        }
    }
}