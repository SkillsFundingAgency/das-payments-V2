using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class FunctionalSkillRequiredPaymentEvent : RequiredPaymentEvent
    {
        public FunctionalSkillType Type { get; set; }
    }
}