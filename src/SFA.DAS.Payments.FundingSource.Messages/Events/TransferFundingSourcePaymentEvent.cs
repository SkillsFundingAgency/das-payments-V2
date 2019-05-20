namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    public class TransferFundingSourcePaymentEvent : FundingSourcePaymentEvent
    {
        public string AgreementId { get; set; }
    }
}