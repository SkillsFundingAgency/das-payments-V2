using SFA.DAS.Payments.Messages.Common;

namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    public class LevyFundingSourcePaymentEvent : FundingSourcePaymentEvent, ILeafLevelMessage
    {
        public string AgreementId { get; set; }
    }
}