using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Application
{
    public interface IFunctionalSkillsEarningService
    {
        IncentivePaymentDueEvent[] CreatePaymentsDue(FunctionalSkillEarningsEvent message);
    }
}