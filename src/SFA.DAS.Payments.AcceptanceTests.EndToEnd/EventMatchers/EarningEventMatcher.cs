﻿using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using OnProgrammeEarning = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.OnProgrammeEarning;
using IncentiveEarning = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.IncentiveEarning;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public static class EarningEventMatcher
    {
        public static Tuple<bool, string> MatchEarnings(IList<OnProgrammeEarning> expectedPeriods, TestSession testSession)
        {
            var sessionEarnings = ApprenticeshipContractType2EarningEventHandler.ReceivedEvents
                .Where(e => e.Ukprn == testSession.Ukprn && e.JobId == testSession.JobId)
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
                if (!learningEarnings.Any(earning => expected.DeliveryCalendarPeriod.Period == earning.period.Period &&
                                                     testSession.GetLearner(expected.LearnerId).LearnRefNumber == earning.earning.earningEvent.Learner.ReferenceNumber &&
                                                     expected.OnProgramme == earning.period.Amount))
                    return new Tuple<bool, string>(false, $"Failed to find on-prog (learning) earning: {expected.DeliveryPeriod} ({expected.DeliveryCalendarPeriod.Name}), amount: {expected.OnProgramme}");

                if (!completionEarnings.Any(earning => expected.DeliveryCalendarPeriod.Period == earning.period.Period &&
                                                       testSession.GetLearner(expected.LearnerId).LearnRefNumber == earning.earning.earningEvent.Learner.ReferenceNumber &&
                                                       expected.Completion == earning.period.Amount))
                    return new Tuple<bool, string>(false, $"Failed to find completion earning: {expected.DeliveryPeriod} ({expected.DeliveryCalendarPeriod.Name}), amount: {expected.Completion}");

                if (!balancingEarnings.Any(earning => expected.DeliveryCalendarPeriod.Period == earning.period.Period &&
                                                      testSession.GetLearner(expected.LearnerId).LearnRefNumber == earning.earning.earningEvent.Learner.ReferenceNumber &&
                                                      expected.Balancing == earning.period.Amount))
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

        public static Tuple<bool, string> MatchIncentives(IList<IncentiveEarning> expectedPeriods,
            TestSession testSession)
        {
            var sessionEarnings = ApprenticeshipContractType2EarningEventHandler.ReceivedEvents
                .Where(e => e.Ukprn == testSession.Ukprn && e.JobId == testSession.JobId)
                .ToList();

            if (sessionEarnings.Any(earning => earning.IncentiveEarnings != null))
            {
                var earnings = sessionEarnings
                    .SelectMany(earning => earning.IncentiveEarnings, (earningEvent, incentiveEarning) => (
                        earningEvent: earningEvent as ApprenticeshipContractTypeEarningsEvent,
                        incentiveEarning: incentiveEarning
                    ))
                    .SelectMany(earning => earning.incentiveEarning.Periods,
                        (earning, period) => (earningEvent: earning.earningEvent,
                            incentiveEarning: earning.incentiveEarning, period: period))
                    .ToList();

                var first16To18EmployerIncentiveEarnings = earnings
                    .Where(earning => earning.incentiveEarning.Type == IncentiveType.First16To18EmployerIncentive)
                    .ToList();
                var first16To18ProviderIncentiveEarnings = earnings
                    .Where(earning => earning.incentiveEarning.Type == IncentiveType.First16To18ProviderIncentive)
                    .ToList();
                var second16To18EmployerIncentiveEarnings = earnings
                    .Where(earning => earning.incentiveEarning.Type == IncentiveType.Second16To18EmployerIncentive)
                    .ToList();
                var second16To18ProviderIncentiveEarnings = earnings
                    .Where(earning => earning.incentiveEarning.Type == IncentiveType.Second16To18ProviderIncentive)
                    .ToList();
                var onProgramme16To18FrameworkUpliftEarnings = earnings
                    .Where(earning => earning.incentiveEarning.Type == IncentiveType.OnProgramme16To18FrameworkUplift)
                    .ToList();
                var completion16To18FrameworkUpliftEarnings = earnings
                    .Where(earning => earning.incentiveEarning.Type == IncentiveType.Completion16To18FrameworkUplift)
                    .ToList();
                var balancing16To18FrameworkUpliftEarnings = earnings
                    .Where(earning => earning.incentiveEarning.Type == IncentiveType.Balancing16To18FrameworkUplift)
                    .ToList();
                var firstDisadvantagePaymentEarnings = earnings
                    .Where(earning => earning.incentiveEarning.Type == IncentiveType.FirstDisadvantagePayment)
                    .ToList();
                var secondDisadvantagePaymentEarnings = earnings
                    .Where(earning => earning.incentiveEarning.Type == IncentiveType.SecondDisadvantagePayment)
                    .ToList();
                var learningSupportEarnings = earnings
                    .Where(earning => earning.incentiveEarning.Type == IncentiveType.LearningSupport)
                    .ToList();

                foreach (var expected in expectedPeriods)
                {
                    if (!CheckPeriodIncentiveEarningsAreValid(testSession, first16To18EmployerIncentiveEarnings,
                        expected,
                        () => expected.First16To18EmployerIncentive, "First16To18EmployerIncentive",
                        out var first16To18Employer))
                        return first16To18Employer;
                    if (!CheckPeriodIncentiveEarningsAreValid(testSession, first16To18ProviderIncentiveEarnings,
                        expected,
                        () => expected.First16To18ProviderIncentive, "First16To18ProviderIncentive",
                        out var first16To18Provider))
                        return first16To18Provider;
                    if (!CheckPeriodIncentiveEarningsAreValid(testSession, second16To18EmployerIncentiveEarnings,
                        expected,
                        () => expected.Second16To18EmployerIncentive, "Second16To18EmployerIncentive",
                        out var second16To18Employer))
                        return second16To18Employer;
                    if (!CheckPeriodIncentiveEarningsAreValid(testSession, second16To18ProviderIncentiveEarnings,
                        expected,
                        () => expected.Second16To18ProviderIncentive, "Second16To18ProviderIncentive",
                        out var second16To18Provider))
                        return second16To18Provider;
                    if (!CheckPeriodIncentiveEarningsAreValid(testSession, onProgramme16To18FrameworkUpliftEarnings,
                        expected, () => expected.OnProgramme16To18FrameworkUplift, "OnProgramme16To18FrameworkUplift",
                        out var onProgramme16To18))
                        return onProgramme16To18;
                    if (!CheckPeriodIncentiveEarningsAreValid(testSession, completion16To18FrameworkUpliftEarnings,
                        expected, () => expected.Completion16To18FrameworkUplift, "Completion16To18FrameworkUplift",
                        out var completion16To18))
                        return completion16To18;
                    if (!CheckPeriodIncentiveEarningsAreValid(testSession, balancing16To18FrameworkUpliftEarnings,
                        expected,
                        () => expected.Balancing16To18FrameworkUplift, "Balancing16To18FrameworkUplift",
                        out var balancing16To18)) return balancing16To18;
                    if (!CheckPeriodIncentiveEarningsAreValid(testSession, firstDisadvantagePaymentEarnings, expected,
                        () => expected.FirstDisadvantagePayment, "FirstDisadvantagePayment", out var firstDisadvantage))
                        return firstDisadvantage;
                    if (!CheckPeriodIncentiveEarningsAreValid(testSession, secondDisadvantagePaymentEarnings, expected,
                        () => expected.SecondDisadvantagePayment, "SecondDisadvantagePayment",
                        out var secondDisadvantage))
                        return secondDisadvantage;
                    if (!CheckPeriodIncentiveEarningsAreValid(testSession, learningSupportEarnings, expected,
                        () => expected.LearningSupport, "LearningSupport", out var learningSupport))
                        return learningSupport;
                }
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static bool CheckPeriodIncentiveEarningsAreValid(TestSession testSession,
            List<(ApprenticeshipContractTypeEarningsEvent earningEvent, Model.Core.Incentives.IncentiveEarning incentiveEarning, EarningPeriod
                period)> incentiveEarnings, IncentiveEarning expected, Func<decimal> expectedEarningValue, string earningName, out Tuple<bool, string> result)
        {
            var expectedValue = expectedEarningValue();

            if (!incentiveEarnings.Any(earning =>
                expected.DeliveryCalendarPeriod.Period == earning.period.Period &&
                testSession.GetLearner(expected.LearnerId).LearnRefNumber ==
                earning.earningEvent.Learner.ReferenceNumber &&
                expectedValue == earning.period.Amount))
            {
                result = new Tuple<bool, string>(false,
                    $"Failed to find incentive ({earningName}) earning: {expected.DeliveryPeriod} ({expected.DeliveryCalendarPeriod.Name}), amount: {expectedValue}");
                return false;
            }

            result = null;
            return true;
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
