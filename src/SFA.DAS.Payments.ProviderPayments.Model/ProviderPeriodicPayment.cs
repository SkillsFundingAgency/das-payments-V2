using SFA.DAS.Payments.Model.Core;
using System;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.FundingSource.Model.Enum;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.ProviderPayments.Model
{
    public class ProviderPeriodicPayment:IPaymentsMessage
    {
        public string PriceEpisodeIdentifier { get; set; }

        public decimal AmountDue { get; set; }

        public CalendarPeriod CollectionPeriod { get; set; }

        public CalendarPeriod DeliveryPeriod { get; set; }

        public DateTimeOffset EventTime { get; set; }

        public long Ukprn { get; set; }

        public Learner Learner { get; set; }

        public LearningAim LearningAim { get; set; }

        public string JobId { get; set; }

        public byte ContractType { get; set; }

        public decimal SfaContributionPercentage { get; set; }

        public OnProgrammeEarningType OnProgrammeEarningType { get; set; }

        public FundingSourceType FundingSourceType { get; set; }

        public DateTime SubmissionDate { get; set; }
    }
}
