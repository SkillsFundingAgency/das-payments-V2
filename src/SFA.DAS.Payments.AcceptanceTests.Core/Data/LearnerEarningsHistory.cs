using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Tests.Core;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class LearnerEarningsHistory
    {
        private readonly AdditionalIlrData additionalIlrData;

        private readonly List<Earning> previousEarnings;

        public LearnerEarningsHistory(AdditionalIlrData additionalIlrData, IEnumerable<Earning> previousEarnings)
        {
            this.additionalIlrData = additionalIlrData ?? new AdditionalIlrData();
            this.previousEarnings = previousEarnings?.ToList() ?? new List<Earning>();
        }

        public string CollectionYear => additionalIlrData.HistoryPeriod.ToDate().Year.ToString();

        public int CollectionPeriod => additionalIlrData.HistoryPeriod.ToMonthPeriod();

        public int EmployerId => additionalIlrData.Employer.ToEmployerAccountId();

        public decimal? OnProgrammeEarningsToDate
        {
            get
            {
                var previouslyPaidEarnings =
                    previousEarnings?.Where(pe =>
                        pe.DeliveryPeriod.ToDate().Year <= Convert.ToInt32(CollectionYear) &&
                        pe.DeliveryPeriod.ToMonthPeriod() <= CollectionPeriod).ToList() ?? new List<Earning>();

                var onProgrammeEarningsToDate = previouslyPaidEarnings.Sum(x => x.OnProgramme);

                return onProgrammeEarningsToDate; 
            }
        }

        public decimal? TotalEarningsToDate => OnProgrammeEarningsToDate + BalancingEarningsToDate + CompletionEarningsToDate;

        public decimal? BalancingEarningsToDate
        {
            get { return previousEarnings.Sum(x => x.Balancing); }
        }

        public decimal? CompletionEarningsToDate
        {
            get { return previousEarnings.Sum(x => x.Completion); }
        }

        public DateTime? UpToEndDate(string originalStartDate)
        {
            return originalStartDate.ToDate().Add(HistoryActualDuration(originalStartDate));
        }

        public int TrainingDaysCompleted(string originalStartDate)
        {
            return HistoryActualDuration(originalStartDate).Days + 1;
        }

        private TimeSpan HistoryActualDuration(string originalStartDate)
        {
            if (string.IsNullOrWhiteSpace(originalStartDate) || string.IsNullOrWhiteSpace(additionalIlrData.ActualDuration))
            {
                return new TimeSpan();
            }

            return additionalIlrData.ActualDuration.ToTimeSpan(originalStartDate).HasValue
                       ? additionalIlrData.ActualDuration.ToTimeSpan(originalStartDate).Value
                       : new TimeSpan();
        }
    }
}
