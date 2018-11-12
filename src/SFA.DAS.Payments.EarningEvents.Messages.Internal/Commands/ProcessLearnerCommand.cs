using System;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Messages.Core.Commands;

namespace SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands
{
    public class ProcessLearnerCommand: PaymentsCommand
    {
        public long Ukprn { get; set; }
        public string CollectionYear { get; set; }
        public int CollectionPeriod { get; set; }
        public FM36Learner Learner { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
    }
}