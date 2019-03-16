namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    public class LevyFundingSourcePaymentEvent : FundingSourcePaymentEvent
    {
        public string AgreementId { get; set; }
        public long EmployerAccountId { get; set; }
    }
}