using System;
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
        public static (bool pass, string reason) MatchEarnings(IList<OnProgrammeEarning> expectedPeriods,
            TestSession testSession)
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
                                                     testSession.GetLearner(expected.LearnerId).LearnRefNumber ==
                                                     earning.earning.earningEvent.Learner.ReferenceNumber &&
                                                     expected.OnProgramme == earning.period.Amount))
                    return (false,
                        $"Failed to find on-prog (learning) earning: {expected.DeliveryPeriod} ({expected.DeliveryCalendarPeriod.Name}), amount: {expected.OnProgramme}");

                if (!completionEarnings.Any(earning =>
                    expected.DeliveryCalendarPeriod.Period == earning.period.Period &&
                    testSession.GetLearner(expected.LearnerId).LearnRefNumber ==
                    earning.earning.earningEvent.Learner.ReferenceNumber &&
                    expected.Completion == earning.period.Amount))
                    return (false,
                        $"Failed to find completion earning: {expected.DeliveryPeriod} ({expected.DeliveryCalendarPeriod.Name}), amount: {expected.Completion}");

                if (!balancingEarnings.Any(earning => expected.DeliveryCalendarPeriod.Period == earning.period.Period &&
                                                      testSession.GetLearner(expected.LearnerId).LearnRefNumber ==
                                                      earning.earning.earningEvent.Learner.ReferenceNumber &&
                                                      expected.Balancing == earning.period.Amount))
                    return (false,
                        $"Failed to find balancing earning: {expected.DeliveryPeriod} ({expected.DeliveryCalendarPeriod.Name}), amount: {expected.Balancing}");
            }

            return (true, string.Empty);
            //TOOD: Figure what nothing extra should be doing
            //var nothingExtra = receivedPeriods.Count == matchedPeriods.Length;
            //var reason = new List<string>();
            //if (!allFound)
            //    reason.Add($"Did not find {expectedPeriods.Count - matchedPeriods.Length} out of {expectedPeriods.Count} expected earnings");
            //if (!nothingExtra)
            //    reason.Add($"Found {receivedPeriods.Count - matchedPeriods.Length} unexpected earnings");

            //return new Tuple<bool, string>(allFound && nothingExtra, string.Join(" and ", reason));
        }

        public static (bool pass, string reason) MatchIncentives(IList<IncentiveEarning> expectedPeriods,
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

                foreach (var incentiveType in IncentiveTypes)
                {
                    var incentiveEarnings = earnings
                        .Where(earning => earning.incentiveEarning.Type == incentiveType)
                        .ToList();

                    if (incentiveEarnings.Any())
                    {
                        foreach (var expected in expectedPeriods)
                        {
                            var expectedValue = expected.GetIncentiveTypeValue(incentiveType);

                            if (!incentiveEarnings.Any(earning =>
                                expected.DeliveryCalendarPeriod.Period == earning.period.Period &&
                                testSession.GetLearner(expected.LearnerId).LearnRefNumber ==
                                earning.earningEvent.Learner.ReferenceNumber &&
                                expectedValue == earning.period.Amount))
                            {
                                return (false,
                                     $"Failed to find incentive ({incentiveType.ToString()}) earning: {expected.DeliveryPeriod} ({expected.DeliveryCalendarPeriod.Name}), amount: {expectedValue}");
                            }
                        }
                    }
                }

            }

            return (true, string.Empty);
        }

        private static readonly IncentiveType[] IncentiveTypes =
        {
            IncentiveType.First16To18EmployerIncentive,
            IncentiveType.First16To18ProviderIncentive,
            IncentiveType.Second16To18EmployerIncentive,
            IncentiveType.Second16To18ProviderIncentive,
            IncentiveType.OnProgramme16To18FrameworkUplift,
            IncentiveType.Completion16To18FrameworkUplift,
            IncentiveType.Balancing16To18FrameworkUplift,
            IncentiveType.FirstDisadvantagePayment,
            IncentiveType.SecondDisadvantagePayment,
            IncentiveType.LearningSupport
        };

        private static decimal GetIncentiveTypeValue(this IncentiveEarning incentiveEarning, IncentiveType type)
        {
            switch (type)
            {
                case IncentiveType.First16To18EmployerIncentive:
                    return incentiveEarning.First16To18EmployerIncentive;
                case IncentiveType.First16To18ProviderIncentive:
                    return incentiveEarning.First16To18ProviderIncentive;
                case IncentiveType.Second16To18EmployerIncentive:
                    return incentiveEarning.Second16To18EmployerIncentive;
                case IncentiveType.Second16To18ProviderIncentive:
                    return incentiveEarning.Second16To18ProviderIncentive;
                case IncentiveType.OnProgramme16To18FrameworkUplift:
                    return incentiveEarning.OnProgramme16To18FrameworkUplift;
                case IncentiveType.Completion16To18FrameworkUplift:
                    return incentiveEarning.Completion16To18FrameworkUplift;
                case IncentiveType.Balancing16To18FrameworkUplift:
                    return incentiveEarning.Balancing16To18FrameworkUplift;
                case IncentiveType.FirstDisadvantagePayment:
                    return incentiveEarning.FirstDisadvantagePayment;
                case IncentiveType.SecondDisadvantagePayment:
                    return incentiveEarning.SecondDisadvantagePayment;
                case IncentiveType.LearningSupport:
                    return incentiveEarning.LearningSupport;
                default:
                    return default(decimal);
            }
        }
    }
}