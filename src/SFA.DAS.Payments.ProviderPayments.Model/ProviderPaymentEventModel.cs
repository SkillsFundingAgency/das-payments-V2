using System;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.Model
{
    public class ProviderPaymentEventModel: PeriodisedPaymentsEventModel
    {
        public long Id { get; set; }
        public Guid FundingSourceId { get; set; }
        public FundingSourceType FundingSource { get; set; }
        //TODO: Get rid of the CalenderPeriod rubbish and then fix the schema - only needs CollectionYear, CollectionPeriod & DeliveryPeriod
    }
}