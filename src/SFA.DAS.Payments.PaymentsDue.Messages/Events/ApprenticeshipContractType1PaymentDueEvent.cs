namespace SFA.DAS.Payments.PaymentsDue.Messages.Events
{
    public class ApprenticeshipContractType1PaymentDueEvent : ApprenticeshipContractTypePaymentDueEvent
    {
        public int AgreementId { get; set; }

        public int CommitmentId { get; set; }

        public int EmployerAccountId { get; set; }
    }
}