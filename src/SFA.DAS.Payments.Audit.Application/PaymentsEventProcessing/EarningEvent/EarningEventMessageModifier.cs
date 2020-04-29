using System;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;
using SFA.DAS.Payments.Messaging.Serialization;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent
{
    public class EarningEventMessageModifier : IApplicationMessageModifier
    {
        private readonly IEarningEventMapper mapper;

        public EarningEventMessageModifier(IEarningEventMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public object Modify(object applicationMessage)
        {
            var earningEvent = applicationMessage as EarningEvents.Messages.Events.EarningEvent;
            return earningEvent == null ? applicationMessage : mapper.Map(earningEvent);
        }
    }
}