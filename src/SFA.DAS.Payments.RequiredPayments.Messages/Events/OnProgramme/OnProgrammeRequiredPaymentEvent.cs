namespace SFA.DAS.Payments.RequiredPayments.Messages.Events.OnProgramme
{
    public class OnProgrammeRequiredPaymentEvent : RequiredPaymentEvent
    {
        public decimal SfaContributionPercentage { get; set; }
    }
}