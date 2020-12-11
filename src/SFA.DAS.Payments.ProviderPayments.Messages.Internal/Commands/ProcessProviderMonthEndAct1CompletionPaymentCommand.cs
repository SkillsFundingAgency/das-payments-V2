using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands
{
    public class ProcessProviderMonthEndAct1CompletionPaymentCommand: PaymentsCommand
    {
        public CollectionPeriod CollectionPeriod { get; set; }
        public long Ukprn { get; set; }
    }
}