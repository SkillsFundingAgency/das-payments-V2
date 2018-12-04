using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using FunctionalSkillEarning = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.FunctionalSkillEarning;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class FunctionalSkillEarningEventMatcher : BaseMatcher<FunctionalSkillEarningsEvent>
    {
        private readonly TestSession testSession;
        private readonly CalendarPeriod collectionPeriod;
        private readonly IList<FunctionalSkillEarning> earningSpecs;
        private readonly IList<FM36Learner> learnerSpecs;

        public FunctionalSkillEarningEventMatcher(IList<FunctionalSkillEarning> earningSpecs, TestSession testSession, CalendarPeriod collectionPeriod, IList<FM36Learner> learnerSpecs)
        {
            this.earningSpecs = earningSpecs;
            this.testSession = testSession;
            this.collectionPeriod = collectionPeriod;
            this.learnerSpecs = learnerSpecs;
        }

        protected override IList<FunctionalSkillEarningsEvent> GetActualEvents()
        {
            return FunctionalSkillEarningsEventHandler.ReceivedEvents.Where(e => e.JobId == testSession.JobId 
                                                                                 && e.CollectionPeriod.Name == collectionPeriod.Name
                                                                                 && e.Ukprn == testSession.Ukprn).ToList();
        }

        protected override IList<FunctionalSkillEarningsEvent> GetExpectedEvents()
        {
            var result = new List<FunctionalSkillEarningsEvent>();
            var periodNames = earningSpecs.Select(e => e.DeliveryCalendarPeriod.Name).Distinct().ToList();

            foreach (var earning in earningSpecs)
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
                    CollectionPeriod = collectionPeriod,
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
                var earningSpec = earningSpecs.Single(s => s.DeliveryCalendarPeriod.Name == calendarPeriod.Name && s.LearnerId == leanerId);
                var priceEpisode = FindPriceEpisode(leanerId, periodName);
            
                var onProgPeriod = new EarningPeriod
                {
                    Period = calendarPeriod.Period,
                    Amount = earningSpec.OnProgrammeMathsAndEnglish,
                    PriceEpisodeIdentifier = priceEpisode.PriceEpisodeIdentifier
                };

                var balancingPeriod = new EarningPeriod
                {
                    Period = calendarPeriod.Period,
                    Amount = earningSpec.BalancingMathsAndEnglish,
                    PriceEpisodeIdentifier = priceEpisode.PriceEpisodeIdentifier
                };

                onProgPeriods.Add(onProgPeriod);
                balancingPeriods.Add(balancingPeriod);
            }

            return (onProgPeriods, balancingPeriods);
        }

        private PriceEpisode FindPriceEpisode(string leanerId, string periodName)
        {
            // find first price episode with non-zero value for a period, otherwise return first one
            var period = int.Parse(periodName.Substring(6, 2));
            var learnerSpec = learnerSpecs.Single(l => l.LearnRefNumber == testSession.GetLearner(leanerId).LearnRefNumber);
            var nonZeroEpisode = learnerSpec.PriceEpisodes.SingleOrDefault(pe => pe.PriceEpisodePeriodisedValues.Any(pepv => pepv.GetValue(period).GetValueOrDefault(0) > 0));
            return nonZeroEpisode ?? learnerSpec.PriceEpisodes.First();
        }

        protected override bool Match(FunctionalSkillEarningsEvent expectedEvent, FunctionalSkillEarningsEvent actualEvent)
        {
            if (expectedEvent.Earnings.Count != actualEvent.Earnings.Count ||
                expectedEvent.CollectionPeriod.Name != actualEvent.CollectionPeriod.Name ||
                expectedEvent.EarningYear != actualEvent.EarningYear ||
                expectedEvent.Learner.ReferenceNumber != actualEvent.Learner.ReferenceNumber ||
                expectedEvent.Learner.Uln != actualEvent.Learner.Uln ||
                expectedEvent.LearningAim.Reference != actualEvent.LearningAim.Reference ||
                expectedEvent.LearningAim.FundingLineType != actualEvent.LearningAim.Reference ||
                expectedEvent.LearningAim.FrameworkCode != actualEvent.LearningAim.FrameworkCode ||
                expectedEvent.LearningAim.PathwayCode != actualEvent.LearningAim.PathwayCode ||
                expectedEvent.LearningAim.ProgrammeType != actualEvent.LearningAim.ProgrammeType ||
                expectedEvent.LearningAim.StandardCode != actualEvent.LearningAim.StandardCode)
                return false;

            foreach (var expectedEarning in expectedEvent.Earnings)
            {
                var actualEarning = actualEvent.Earnings.FirstOrDefault(a => a.Type == expectedEarning.Type);

                if (actualEarning == null || actualEarning.Periods.Count != expectedEarning.Periods.Count)
                    return false;

                for (var i = 0; i < expectedEarning.Periods.Count; i++)
                {
                    if (expectedEarning.Periods[i].Amount != actualEarning.Periods[i].Amount ||
                        expectedEarning.Periods[i].PriceEpisodeIdentifier != actualEarning.Periods[i].PriceEpisodeIdentifier ||
                        expectedEarning.Periods[i].Period != actualEarning.Periods[i].Period)
                        return false;
                }
            }

            return true;
        }
    }
}