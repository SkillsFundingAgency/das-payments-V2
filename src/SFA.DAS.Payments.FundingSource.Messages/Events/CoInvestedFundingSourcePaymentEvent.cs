using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    public  class CoInvestedFundingSourcePaymentEvent : FundingSourcePaymentEvent
    {
        public decimal SfaContributionPercentage { get; set; }

        public OnProgrammeEarningType OnProgrammeEarningType { get; set; }
    }
}