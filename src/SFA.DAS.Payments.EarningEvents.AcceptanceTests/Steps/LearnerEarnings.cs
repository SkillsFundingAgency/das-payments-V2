using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using DateTime = System.DateTime;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    public class LearnerEarnings
    {
        public string PriceEpisodeIdentifier { get; set; }
        public string Periods { get; set; }
        public string CompletionStatus { get; set; }
        public decimal? TotalTrainingPrice { get; set; }
        public decimal? TotalAssessmentPrice { get; set; }
        public decimal? AgreedPrice => TotalTrainingPrice + TotalAssessmentPrice;
        public decimal? InstallmentAmount => (AgreedPrice * (decimal) .8) / NumberOfInstallments;
        public decimal? CompletionAmount => AgreedPrice * (decimal) .2;
        public int AimSequenceNumber { get; set; }
        public string AimReference { get; set; }
        public string StandardCode { get; set; }
        public string ProgrammeType { get; set; }
        public string FundingLineType { get; set; }
        public decimal? BalancingPayment { get; set; }
        public string LearnerStartDate { get; set; }
        public int NumberOfInstallments { get; set; }
        public string EpisodeStartDate { get; set; }
        public List<string> GetPeriodsList()
        {
            if (!Periods.Contains("-"))
                return new List<string> { Periods };
            var parts = Periods
                .Split('-')
                .Select(int.Parse)
                .ToList();
            return Enumerable.Range(parts.First(), parts.Last() - parts.First() + 1)
                .Select(period => period.ToString())
                .ToList();
        }
    }
}