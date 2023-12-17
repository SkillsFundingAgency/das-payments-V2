using SFA.DAS.Payments.Messages.Common.Commands;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.FundingSource.Messages.Internal.Commands
{
    public class ProcessSubmissionDeletion: PaymentsCommand
    {
        public CollectionPeriod CollectionPeriod { get; set; }

        public long AccountId { get; set; }
    }
}
