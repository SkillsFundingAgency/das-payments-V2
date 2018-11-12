using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using OnProgrammeEarning = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.OnProgrammeEarning;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public static class EarningEventMatcher
    {
        public static Tuple<bool, string> MatchEarnings(IList<OnProgrammeEarning> expectedPeriods, long ukprn, string learnerReference, long jobId)
        {
            var sessionEarnings = ApprenticeshipContractType2EarningEventHandler.ReceivedEvents
                .Where(e => e.Ukprn == ukprn && e.Learner.ReferenceNumber.Equals(learnerReference) && e.JobId == jobId)
                .ToList();

            var earnings = sessionEarnings
                .SelectMany(earning => earning.OnProgrammeEarnings, (earningEvent, onProgEarning) => new
                {
                    earningEvent,
                    onProgEarning
                })
                .SelectMany(earning => earning.onProgEarning.Periods, (earning, period) => new { earning, period })
                .ToList();

            var learningEarnings = earnings
                .Where(earning => earning.earning.onProgEarning.Type == OnProgrammeEarningType.Learning)
                .ToList();
            var completionEarnings = earnings
                .Where(earning => earning.earning.onProgEarning.Type == OnProgrammeEarningType.Completion)
                .ToList();
            var balancingEarnings = earnings
                .Where(earning => earning.earning.onProgEarning.Type == OnProgrammeEarningType.Balancing)
                .ToList();

            foreach (var expected in expectedPeriods)
            {
                if (!learningEarnings.Any(earning => expected.DeliveryCalendarPeriod.Period == earning.period.Period && expected.OnProgramme == earning.period.Amount))
                    return new Tuple<bool, string>(false, $"Failed to find on-prog (learning) earning: {expected.DeliveryPeriod} ({expected.DeliveryCalendarPeriod.Name}), amount: {expected.OnProgramme}");

                if (!completionEarnings.Any(earning => expected.DeliveryCalendarPeriod.Period == earning.period.Period && expected.Completion == earning.period.Amount))
                    return new Tuple<bool, string>(false, $"Failed to find completion earning: {expected.DeliveryPeriod} ({expected.DeliveryCalendarPeriod.Name}), amount: {expected.Completion}");

                if (!balancingEarnings.Any(earning => expected.DeliveryCalendarPeriod.Period == earning.period.Period && expected.Balancing == earning.period.Amount))
                    return new Tuple<bool, string>(false, $"Failed to find balancing earning: {expected.DeliveryPeriod} ({expected.DeliveryCalendarPeriod.Name}), amount: {expected.Balancing}");
            }

            return new Tuple<bool, string>(true, string.Empty);
            //TOOD: Figure what nothing extra should be doing
            //var nothingExtra = receivedPeriods.Count == matchedPeriods.Length;
            //var reason = new List<string>();
            //if (!allFound)
            //    reason.Add($"Did not find {expectedPeriods.Count - matchedPeriods.Length} out of {expectedPeriods.Count} expected earnings");
            //if (!nothingExtra)
            //    reason.Add($"Found {receivedPeriods.Count - matchedPeriods.Length} unexpected earnings");

            //return new Tuple<bool, string>(allFound && nothingExtra, string.Join(" and ", reason));
        }

        //private static Dictionary<string, OnProgrammeEarning> ConvertToOnProgEarning(ApprenticeshipContractType2EarningEvent[] sessionEarnings)
        //{
        //    var receivedPeriods = sessionEarnings
        //        .SelectMany(e => e.OnProgrammeEarnings
        //            .SelectMany(pe => pe.Periods.Select(p => p.Period)))
        //        .Distinct()
        //        .ToDictionary(p => p.Name, p => new OnProgrammeEarning
        //        {
        //            DeliveryCalendarPeriod = p
        //        });

        //    foreach (var receivedEarning in sessionEarnings.SelectMany(e => e.OnProgrammeEarnings))
        //    {
        //        foreach (var period in receivedEarning.Periods)
        //        {
        //            var onProg = receivedPeriods[period.Period.Name];
        //            switch (receivedEarning.Type)
        //            {
        //                case OnProgrammeEarningType.Learning:
        //                    onProg.OnProgramme = period.Amount;
        //                    break;
        //                case OnProgrammeEarningType.Balancing:
        //                    onProg.Balancing = period.Amount;
        //                    break;
        //                case OnProgrammeEarningType.Completion:
        //                    onProg.Completion = period.Amount;
        //                    break;
        //                default:
        //                    throw new NotSupportedException("Unknown earning type " + receivedEarning.Type);
        //            }
        //        }
        //    }

        //    return receivedPeriods;
        //}
    }
}
