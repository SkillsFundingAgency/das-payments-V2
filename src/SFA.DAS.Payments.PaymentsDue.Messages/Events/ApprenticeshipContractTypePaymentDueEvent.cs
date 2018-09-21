using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.PaymentsDue.Messages.Events
{
    public class ApprenticeshipContractTypePaymentDueEvent : PaymentDueEvent
    {
        public decimal SfaContributionPercentage { get; set; }

        public OnProgrammeEarningType Type { get; set; }
    }
}