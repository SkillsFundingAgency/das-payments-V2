using System;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Commands;

namespace SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands
{
    public class ProcessLearnerCommand: PaymentsCommand, IMonitoredMessage
    {

        public long Ukprn { get; set; }
        public short CollectionYear { get; set; }
        public int CollectionPeriod { get; set; }
        public FM36Learner Learner { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public string IlrFileName { get; set; }
    }
}