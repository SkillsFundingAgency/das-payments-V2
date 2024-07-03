using System;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.Model
{
    public class ProviderPaymentEventModel: PeriodisedPaymentsEventModel
    {
        public long Id { get; set; }
        public Guid FundingSourceId { get; set; }
        public Guid? RequiredPaymentEventId { get; set; }
        public Guid? ClawbackSourcePaymentEventId { get; set; }
        public FundingSourceType FundingSource { get; set; }
        public string ReportingAimFundingLineType { get; set; }
        public int? AgeAtStartOfLearning { get; set; }
        public FundingPlatformType? FundingPlatformType { get; set;}
    }
}