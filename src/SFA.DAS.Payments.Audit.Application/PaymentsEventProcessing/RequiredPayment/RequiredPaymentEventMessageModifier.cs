using System;
using SFA.DAS.Payments.Audit.Application.Mapping.RequiredPaymentEvents;
using SFA.DAS.Payments.Messaging.Serialization;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.RequiredPayment
{
    public class RequiredPaymentEventMessageModifier : IApplicationMessageModifier
    {
        private readonly IRequiredPaymentEventMapper mapper;

        public RequiredPaymentEventMessageModifier(IRequiredPaymentEventMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public object Modify(object applicationMessage)
        {
            var requiredPaymentEvent = applicationMessage as IRequiredPaymentEvent;
            return requiredPaymentEvent == null ? applicationMessage : mapper.Map(requiredPaymentEvent);
        }
    }
}