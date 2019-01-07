using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Aim
    {
        public Aim()
        { }

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
        }

        CompletionStatus CompletionStatusFromString(string completionStatus)
        {
            switch (completionStatus.ToLower())
            {
                case "break in learning":
                    return CompletionStatus.BreakInLearning;
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
        public string ActualDuration { get; set; }
        public TimeSpan? ActualDurationAsTimespan
        {
            get
            {
                if (string.IsNullOrEmpty(ActualDuration))
                {
                    return null;
                }

                if (ActualDuration.Contains("months"))
                {
                    var months = int.Parse(ActualDuration.Replace("months", string.Empty));
                    return StartDate.ToDate().AddMonths(months) - StartDate.ToDate();
                }
                throw new Exception($"Could not parse ActualDuration: {ActualDuration}");
            }
        }

        public int AimSequenceNumber { get; set; }
        public int ProgrammeType { get; set; }
        public int StandardCode { get; set; }
        public string FundingLineType { get; set; }
        public CompletionStatus CompletionStatus { get; set; }
        public List<Price> PriceEpisodes { get; set; } = new List<Price>();
        public int FrameworkCode { get; set; }
        public int PathwayCode { get; set; }
    }
}
