using SFA.DAS.Payments.Model.Core.Entities;
using System;
using TechTalk.SpecFlow.Assist.Attributes;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Training
    {
        public long Ukprn { get; set; }
        public string LearnerId { get; set; }
        public long Uln { get; set; }
        public string StartDate { get; set; }
        public string TotalTrainingPriceEffectiveDate { get; set; }
        public string TotalAssessmentPriceEffectiveDate { get; set; }
        public string PlannedDuration { get; set; }
        public decimal TotalTrainingPrice { get; set; }
        public decimal TotalAssessmentPrice { get; set; }
        public string ActualDuration { get; set; }
        public int ProgrammeType { get; set; }
        public string CompletionStatus { get; set; }
        public string SfaContributionPercentage { get; set; }
        public decimal? AgreedPrice => TotalTrainingPrice + TotalAssessmentPrice;
        public decimal? InstallmentAmount => (AgreedPrice * 0.8M) / NumberOfInstallments;
        public decimal? CompletionAmount => AgreedPrice * 0.2M;
        public int NumberOfInstallments
        {
            get
            {
                if (!PlannedDuration.Contains("days"))
                {
                    return int.Parse(PlannedDuration.Replace("months", null).Trim());
                }

                var days = int.Parse(PlannedDuration.Replace("days", null).Trim());

                //if the planned duration is less than or equal to 42 days then 1 installment otherwise 1 installment per month
                return days <= 42 ? 1 : Math.Max((int)Math.Ceiling(days / 30.0), 1);
            }
        }

        public int ActualInstallments => int.Parse(ActualDuration.Replace("months", null).Trim());
        public decimal? BalancingPayment { get; set; } // TODO: populate properly
        public ContractType ContractType { get; set; }
        public int AimSequenceNumber { get; set; }
        public string AimReference { get; set; }
        public int StandardCode { get; set; }
        public string FundingLineType { get; set; }
        public int FrameworkCode { get; set; }
        public int PathwayCode { get; set; }

        [TableAliases("[E|e]mployer [C|c]ontribution")]
        public int? Pmr { get; set; }
        [TableAliases("Exemption Code")]
        public int CompletionHoldBackExemptionCode { get; set; }

        public string SmallEmployer { get; set; }

        public string PostcodePrior { get; set; }
        public int? AgeAtStart { get; set; } = 22;
    }
}
