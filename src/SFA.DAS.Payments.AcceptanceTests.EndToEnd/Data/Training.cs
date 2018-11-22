using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class Training
    {
        public string LearnerId { get; set; }
        public int Priority { get; set; }
        public string StartDate { get; set; }
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
        public int NumberOfInstallments => int.Parse(PlannedDuration.Replace("months", null).Trim());
        public int ActualInstallments => string.IsNullOrEmpty(ActualDuration) ? 0 : int.Parse(ActualDuration.Replace("months", null).Trim());
        public decimal? BalancingPayment { get; set; } // TODO: populate properly
        public ContractType ContractType { get; set; }
        public int AimSequenceNumber { get; set; }
        public string AimReference { get; set; }
        public int StandardCode { get; set; }
        public string FundingLineType { get; set; }
    }
}
