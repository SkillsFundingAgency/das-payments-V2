using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using FunctionalSkillEarning = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.FunctionalSkillEarning;
using OnProgrammeEarning = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.OnProgrammeEarning;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class FunctionalSkillEarningEventMatcher : BaseMatcher<FunctionalSkillEarningsEvent>
    {
        private readonly TestSession testSession;
        private readonly CalendarPeriod collectionPeriod;
        private readonly IList<FunctionalSkillEarning> earningSpec;

        public FunctionalSkillEarningEventMatcher(IList<FunctionalSkillEarning> earningSpec)
        {
            this.earningSpec = earningSpec;
        }

        protected override IList<FunctionalSkillEarningsEvent> GetActualEvents()
        {
            throw new NotImplementedException();
        }

        protected override IList<FunctionalSkillEarningsEvent> GetExpectedEvents()
        {
            var result = new List<FunctionalSkillEarningsEvent>();
            var periodNames = earningSpec.Select(e => e.DeliveryCalendarPeriod.Name).Distinct().ToList();

            foreach (var earning in earningSpec)
            {
                var learner = testSession.GetLearner(earning.LearnerId);
                var periods = OnProgPeriods(periodNames, earning.LearnerId);

                var functionalSkillEarnings = new List<Model.Core.Incentives.FunctionalSkillEarning>
                {
                    new Model.Core.Incentives.FunctionalSkillEarning
                    {
                        Type = FunctionalSkillType.OnProgrammeMathsAndEnglish,
                        Periods = periods.onProgPeriods.AsReadOnly()
                    },
                    new Model.Core.Incentives.FunctionalSkillEarning
                    {
                        Type = FunctionalSkillType.BalancingMathsAndEnglish,
                        Periods = periods.balancingPeriods.AsReadOnly()
                    }
                };

                var earningEvent = new FunctionalSkillEarningsEvent
                {
                    Ukprn = testSession.Ukprn,
                    EarningYear = earning.DeliveryCalendarPeriod.Year,
                    Earnings = functionalSkillEarnings,
                    JobId = testSession.JobId,
                    Learner = new Learner
                    {
                        ReferenceNumber = learner.LearnRefNumber,
                        Uln = learner.Uln
                    },
                    LearningAim = new LearningAim
                    {
                        ProgrammeType = learner.Course.ProgrammeType,
                        FrameworkCode = learner.Course.FrameworkCode,
                        PathwayCode = learner.Course.PathwayCode,
                        StandardCode = learner.Course.StandardCode,
                        FundingLineType = learner.Course.FundingLineType,
                        Reference = learner.Course.LearnAimRef
                    }
                };

                result.Add(earningEvent);
            }

            return result;
        }

        private (List<EarningPeriod> onProgPeriods, List<EarningPeriod> balancingPeriods) OnProgPeriods(List<string> periodNames, string leanerId)
        {
            var onProgPeriods = new List<EarningPeriod>();
            var balancingPeriods = new List<EarningPeriod>();

            foreach (var periodName in periodNames)
            {
                var calendarPeriod = new CalendarPeriod(periodName);
                var spec = earningSpec.First(s => s.DeliveryCalendarPeriod.Name == calendarPeriod.Name && s.LearnerId == leanerId);

                var onProgPeriod = new EarningPeriod
                {
                    Period = calendarPeriod.Period,
                    Amount = spec.OnProgrammeMathsAndEnglish,
                    // TODO: find current price episode
                };

                var balancingPeriod = new EarningPeriod
                {
                    Period = calendarPeriod.Period,
                    Amount = spec.BalancingMathsAndEnglish,
                    // TODO: find current price episode
                };

                onProgPeriods.Add(onProgPeriod);
                balancingPeriods.Add(balancingPeriod);
            }

            return (onProgPeriods, balancingPeriods);
        }

        protected override bool Match(FunctionalSkillEarningsEvent expected, FunctionalSkillEarningsEvent actual)
        {
            throw new NotImplementedException();
        }
    }

    public static class OnProgrammeEarningEventMatcher
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
