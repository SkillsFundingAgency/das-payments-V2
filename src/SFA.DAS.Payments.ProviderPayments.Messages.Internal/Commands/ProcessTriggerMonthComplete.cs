using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands
{
    public class ProcessTriggerMonthComplete
    {
        public ProcessTriggerMonthComplete(CollectionPeriod collectionPeriod)
        {
            CollectionPeriod = collectionPeriod;
        }

        public CollectionPeriod CollectionPeriod { get; set; }
    }
}
