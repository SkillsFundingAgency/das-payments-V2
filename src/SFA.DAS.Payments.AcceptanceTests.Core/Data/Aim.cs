using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;

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
            OriginalStartDate = training.OriginalStartDate;
            PlannedDuration = training.PlannedDuration;
            ActualDuration = training.ActualDuration;
            AimSequenceNumber = training.AimSequenceNumber;
            ProgrammeType = training.ProgrammeType;
            StandardCode = training.StandardCode;
            FrameworkCode = training.FrameworkCode;
            PathwayCode = training.PathwayCode;
            FundingLineType = training.FundingLineType;
            CompletionStatus = CompletionStatusFromString(training.CompletionStatus);
            LearningSupportCode = training.LearningSupportCode;
            LearningSupportDateFrom = training.LearningSupportDateFrom;
            LearningSupportDateTo = training.LearningSupportDateTo;
            FundingAdjustmentForPriorLearning = training.FundingAdjustmentForPriorLearning;
            ContractType = training.ContractType;
            Restart = training.Restart;
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
        public string OriginalStartDate { get; set; }
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

        public int? LearningSupportCode { get; set; }
        public string LearningSupportDateFrom { get; set; }
        public string LearningSupportDateTo { get; set; }

        public string FundingAdjustmentForPriorLearning { get; set; }

        public bool Restart { get; set; }

        public bool IsMainAim => AimReference == "ZPROG001";

        public bool IsMathsAndEnglish => MathsAndEnglishAimReferenceCodes.Contains(AimReference);

        public bool HasContractType => (int) ContractType != 0;

        public ContractType ContractType { get; set; }

        private static IEnumerable<string> MathsAndEnglishAimReferenceCodes =>
            new[]
            {
                "50086832",
                "50089638",
                "50091268",
                "50093186",
                "50094695",
                "50098342",
                "50098342",
                "50114979"
            };
    }
}