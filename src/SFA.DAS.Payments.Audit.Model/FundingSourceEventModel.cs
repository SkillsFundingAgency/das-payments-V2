using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Model
{
    public class FundingSourceEventModel : PeriodisedPaymentsEventModel
    {
        public Guid RequiredPaymentEventId { get; set; }
        public FundingSourceType FundingSource { get; set; }
    }
}