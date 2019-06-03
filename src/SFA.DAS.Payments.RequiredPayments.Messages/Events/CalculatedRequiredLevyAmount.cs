namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class CalculatedRequiredLevyAmount : CalculatedRequiredOnProgrammeAmount
    {
        public int Priority { get; set; }
        public string AgreementId { get; set; }
    }
}