using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace blah.Events
{
    public abstract class CoInvestedFundingSourcePaymentEvent : FundingSourcePaymentEvent
    {
        public decimal SfaContributionPercentage { get; set; }

        public OnProgrammeEarningType OnProgrammeEarningType { get; set; }
    }
}