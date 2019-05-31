using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Tests.Core;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Aim
    {
        public Aim()
        {
        }

        public Aim(Training training)
        {
            LearnerId = training.LearnerId;
            AimReference = training.AimReference;
            StartDate = training.StartDate;
            PlannedDuration = training.PlannedDuration;
            ActualDuration = training.ActualDuration;
            AimSequenceNumber = training.AimSequenceNumber;
            ProgrammeType = training.ProgrammeType;
            StandardCode = training.StandardCode;
            FrameworkCode = training.FrameworkCode;
            PathwayCode = training.PathwayCode;
            FundingLineType = training.FundingLineType;
            CompletionStatus = CompletionStatusFromString(training.CompletionStatus);
            PriceEpisodes = new List<Price>(){new Price()
            {
                TotalTrainingPriceEffectiveDate = training.TotalTrainingPriceEffectiveDate,
                TotalTrainingPrice = training.TotalTrainingPrice,
                TotalAssessmentPriceEffectiveDate = training.TotalAssessmentPriceEffectiveDate,
                TotalAssessmentPrice = training.TotalAssessmentPrice,
                ContractType = training.ContractType,
                AimSequenceNumber = training.AimSequenceNumber,
                SfaContributionPercentage = training.SfaContributionPercentage,
                CompletionHoldBackExemptionCode = training.CompletionHoldBackExemptionCode,
                Pmr = training.Pmr
            }};
        }

        CompletionStatus CompletionStatusFromString(string completionStatus)
        {
            switch (completionStatus.ToLower())
            {
                case "break in learning":
                case "planned break":
                    return CompletionStatus.PlannedBreak;
                case "completed":
                    return CompletionStatus.Completed;
                case "continuing":
                    return CompletionStatus.Continuing;
                case "withdrawn":
                    return CompletionStatus.Withdrawn;
                default:
                    throw new Exception($"Could not translate Completion Status: {completionStatus}");
            }
        }

        public string LearnerId { get; set; }
        public string AimReference { get; set; }
        public string StartDate { get; set; }
        public string PlannedDuration { get; set; }
        public TimeSpan? PlannedDurationAsTimespan => PlannedDuration.ToTimeSpan(StartDate);
        public string ActualDuration { get; set; }
        public TimeSpan? ActualDurationAsTimespan => ActualDuration.ToTimeSpan(StartDate);

        public int AimSequenceNumber { get; set; }
        public int ProgrammeType { get; set; }
        public int StandardCode { get; set; }
        public string FundingLineType { get; set; }
        public CompletionStatus CompletionStatus { get; set; }
        public List<Price> PriceEpisodes { get; set; } = new List<Price>();
        public int FrameworkCode { get; set; }
        public int PathwayCode { get; set; }

        public bool IsMainAim => AimReference == "ZPROG001";
    }
}