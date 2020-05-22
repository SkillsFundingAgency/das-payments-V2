using System;
using SFA.DAS.Payments.Audit.Application.Mapping.DataLock;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Messaging.Serialization;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock
{
    public class DataLockEventMessageModifier : IApplicationMessageModifier
    {
        private readonly IDataLockEventMapper mapper;

        public DataLockEventMessageModifier(IDataLockEventMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public object Modify(object applicationMessage)
        {
            var dataLockEvent = applicationMessage as DataLockEvent;
            return dataLockEvent == null ? applicationMessage : mapper.Map(dataLockEvent);
        }
    }
}