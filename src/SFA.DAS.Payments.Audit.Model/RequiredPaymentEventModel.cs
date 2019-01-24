using System;

namespace SFA.DAS.Payments.Audit.Model
{
    public class RequiredPaymentEventModel : PeriodisedPaymentsEventModel
    {
        public Guid EarningEventId { get; set; }
    }
}