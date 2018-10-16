using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Domain;
using SFA.DAS.Payments.Messages.Core.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.Messages
{
    public class ProcessLearnerCommand: PaymentsCommand, IIlrLearnerSubmission
    {
        public long Ukprn { get; set; }
        public string CollectionYear { get; set; }
        public FM36Learner Learner { get; set; }
    }
}