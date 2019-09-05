using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    public class LevyFundingSourcePaymentEvent : FundingSourcePaymentEvent, ILeafLevelMessage
    {
        public string AgreementId { get; set; }
    }
}