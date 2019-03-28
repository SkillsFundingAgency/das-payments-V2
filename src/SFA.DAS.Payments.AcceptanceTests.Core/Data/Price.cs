using System;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core;
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
        public string ResidualTrainingPriceEffectiveDate { get; set; }
        [TableAliases("[E|e]mployer [C|c]ontribution")]
        public int Pmr { get; set; }
        [TableAliases("[L|l][D|d][M|m] [C|c]ode")]
        public int LdmCode { get; set; }

        public DateTime EpisodeStartDate =>
            ResidualAssessmentPrice == 0 && ResidualTrainingPrice == 0
                ? TotalTrainingPriceEffectiveDate.ToDate()
                : ResidualTrainingPriceEffectiveDate.ToDate();

        public decimal TotalTNPPrice =>
            (ResidualTrainingPrice == 0 && ResidualAssessmentPrice == 0)
                ? TotalTrainingPrice + TotalAssessmentPrice
                : ResidualTrainingPrice + ResidualAssessmentPrice;
    }
}
