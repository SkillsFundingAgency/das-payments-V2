using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class CalculatedRequiredCoInvestedAmount : CalculatedRequiredOnProgrammeAmount
    {
        public FundingPlatformType FundingPlatformType { get; set; } = FundingPlatformType.SubmitLearnerData;
    }
}