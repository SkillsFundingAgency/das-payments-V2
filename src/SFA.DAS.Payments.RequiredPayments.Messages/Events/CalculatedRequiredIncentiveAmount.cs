using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class CalculatedRequiredIncentiveAmount: PeriodisedRequiredPaymentEvent, IMonitoredMessage
    {
        public IncentivePaymentType Type { get; set; }
    }
}