namespace SFA.DAS.Payments.RequiredPayments.Messages.Events.OnProgramme
{
    public class ApprenticeshipContractType1RequiredPaymentEvent: OnProgrammeRequiredPaymentEvent
    {
        public long CommitmentId { get; set; }
        public string AgreementId { get; set; }
        public long EmployerAccountId { get; set; }
    }
}