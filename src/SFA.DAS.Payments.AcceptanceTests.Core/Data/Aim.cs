using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Aim
    {
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
