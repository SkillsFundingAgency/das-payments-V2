﻿using System;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow.Assist.Attributes;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Training
    {
        public long Ukprn { get; set; }
        public string LearnerId { get; set; }
        public long Uln { get; set; }
        public string StartDate { get; set; }
        public string TotalTrainingPriceEffectiveDate {  get; set; }
        public string  TotalAssessmentPriceEffectiveDate { get; set; }
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
        public int FrameworkCode { get; set; }
        public int PathwayCode { get; set; }

        [TableAliases("[E|e]mployer [C|c]ontribution")]
        public int? Pmr { get; set; } 
        [TableAliases("Exemption Code")]
        public int CompletionHoldBackExemptionCode { get; set; }
        public string TotalTrainingPriceEffectiveDate { get; set; }
        public string TotalAssessmentPriceEffectiveDate { get; set; }

    }
}
