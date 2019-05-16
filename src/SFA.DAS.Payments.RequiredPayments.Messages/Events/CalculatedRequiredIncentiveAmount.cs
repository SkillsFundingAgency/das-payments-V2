using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class CalculatedRequiredIncentiveAmount: PeriodisedRequiredPaymentEvent
    {
        public IncentivePaymentType Type { get; set; }
    }
}