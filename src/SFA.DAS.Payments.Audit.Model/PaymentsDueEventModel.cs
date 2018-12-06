using System;

namespace SFA.DAS.Payments.Audit.Model
{
    public class PaymentsDueEventModel : PeriodisedPaymentsEventModel
    {
        public Guid EarningEventId { get; set; }
    }
}
