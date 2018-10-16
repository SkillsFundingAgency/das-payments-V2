using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;

namespace SFA.DAS.Payments.EarningEvents.Domain
{
    public interface IIlrLearnerSubmission
    {
        long Ukprn { get; set; }
        string CollectionYear { get; set; }
        string JobId { get; set; }
        FM36Learner Learner { get; set; }
    }
}