using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow.Assist.Attributes;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Price
    {
        public string PriceDetails { get; set; }
        public decimal TotalTrainingPrice { get; set; }
        public string TotalTrainingPriceEffectiveDate { get; set; }
        public decimal TotalAssessmentPrice { get; set; }
        public string TotalAssessmentPriceEffectiveDate { get; set; }
        public string PriceEpisodeId { get; set; }
        public ContractType ContractType { get; set; }
        public int AimSequenceNumber { get; set; }
        public string FundingLineType { get; set; }
        [TableAliases("SFA[ ]?Contribution[ ]?Percentage")]
        public string SfaContributionPercentage { get; set; }
        public decimal ResidualTrainingPrice { get; set; }
        public decimal ResidualAssessmentPrice { get; set; }

    }
}
