using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands
{
    public class ProcessTriggerMonthComplete : IMonitoredMessage
    {
        public ProcessTriggerMonthComplete(CollectionPeriod collectionPeriod)
        {
            CollectionPeriod = collectionPeriod;
        }

        public CollectionPeriod CollectionPeriod { get; set; }
        public long JobId { get; set; }
    }
}
