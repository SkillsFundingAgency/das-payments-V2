namespace SFA.DAS.Payments.ProviderPayments.Messages
{
    public class LevyProviderPaymentEvent : ProviderPaymentEvent
    {
        public long EmployerAccountId { get; set; }
    }
}