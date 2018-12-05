using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class EarningEventMatcher : BaseMatcher<EarningEvent>
    {
        private readonly TestSession testSession;
        private readonly CalendarPeriod collectionPeriod;
        private readonly IList<Earning> earningSpecs;
        private readonly IList<FM36Learner> learnerSpecs;
        private static readonly TransactionType[] onProgTypes = { TransactionType.Learning, TransactionType.Balancing, TransactionType.Completion };
        private static readonly TransactionType[] incentiveTypes = { TransactionType.First16To18EmployerIncentive, TransactionType.First16To18ProviderIncentive, TransactionType.Second16To18EmployerIncentive, TransactionType.Second16To18ProviderIncentive };
        private static readonly TransactionType[] functionalSkillTypes = { TransactionType.OnProgrammeMathsAndEnglish, TransactionType.BalancingMathsAndEnglish };

        public EarningEventMatcher(IList<Earning> earningSpecs, TestSession testSession, CalendarPeriod collectionPeriod, IList<FM36Learner> learnerSpecs)
        {
            this.earningSpecs = earningSpecs;
            this.testSession = testSession;
            this.collectionPeriod = collectionPeriod;
            this.learnerSpecs = learnerSpecs;
        }

        protected override IList<EarningEvent> GetActualEvents()
        {
            return EarningEventHandler.ReceivedEvents.Where(e => e.JobId == testSession.JobId 
                                                                                 && e.CollectionPeriod.Name == collectionPeriod.Name
                                                                                 && e.Ukprn == testSession.Ukprn).ToList();
        }

        protected override IList<EarningEvent> GetExpectedEvents()
        {
            var result = new List<EarningEvent>();
            var learnerIds = earningSpecs.Select(e => e.LearnerId).Distinct().ToList();
            var year = earningSpecs.First().DeliveryCalendarPeriod.Year;

            // TODO: when multiple learning aims introduced, this should be looping through unique aims instead of learners

            foreach (var learnerId in learnerIds)
            {
                var learnerSpec = testSession.GetLearner(learnerId);

                var learner = new Learner
                {
                    ReferenceNumber = learnerSpec.LearnRefNumber,
                    Uln = learnerSpec.Uln
                };

                var learningAim = new LearningAim
                {
                    ProgrammeType = learnerSpec.Course.ProgrammeType,
                    FrameworkCode = learnerSpec.Course.FrameworkCode,
                    PathwayCode = learnerSpec.Course.PathwayCode,
                    StandardCode = learnerSpec.Course.StandardCode,
                    FundingLineType = learnerSpec.Course.FundingLineType,
                    Reference = learnerSpec.Course.LearnAimRef
                };

                var learnerEarningSpecs = earningSpecs.Where(e => e.LearnerId == learnerId).ToList();

                var fullListOfTransactionTypes = learnerEarningSpecs.SelectMany(p => p.Values.Keys).Distinct().ToList();
                var onProgEarnings = fullListOfTransactionTypes.Where(t => onProgTypes.Contains(t)).ToList();
                var incentiveEarnings = fullListOfTransactionTypes.Where(t => incentiveTypes.Contains(t)).ToList();
                var functionalSkillEarnings = fullListOfTransactionTypes.Where(t => functionalSkillTypes.Contains(t)).ToList();

                if (onProgEarnings.Any())
                {
                    var onProgEarning = new ApprenticeshipContractType2EarningEvent
                    {
                        CollectionPeriod = collectionPeriod,
                        Ukprn = testSession.Ukprn,
                        //EarningYear = year,
                        OnProgrammeEarnings = onProgEarnings.Select(tt => new OnProgrammeEarning
                        {
                            Type = (OnProgrammeEarningType)(int)tt,
                            Periods = learnerEarningSpecs.Select(e => new EarningPeriod
                            {
                                Amount = e.Values[tt],
                                Period = e.DeliveryCalendarPeriod.Period,
                                PriceEpisodeIdentifier = FindPriceEpisode(learnerId, e.DeliveryCalendarPeriod.Name).PriceEpisodeIdentifier
                            }).ToList().AsReadOnly()
                        }).ToList().AsReadOnly(),
                        JobId = testSession.JobId,
                        Learner = learner,
                        LearningAim = learningAim
                    };
                    result.Add(onProgEarning);
                }
            }

            return result;
        }

        private PriceEpisode FindPriceEpisode(string leanerId, string periodName)
        {
            // find first price episode with non-zero value for a period, otherwise return first one
            // TODO: this will have to change when multiple aims done
            var period = int.Parse(periodName.Substring(6, 2));
            var learnerSpec = learnerSpecs.Single(l => l.LearnRefNumber == testSession.GetLearner(leanerId).LearnRefNumber);
            var nonZeroEpisode = learnerSpec.PriceEpisodes.SingleOrDefault(pe => pe.PriceEpisodePeriodisedValues.Any(pepv => pepv.GetValue(period).GetValueOrDefault(0) > 0));
            return nonZeroEpisode ?? learnerSpec.PriceEpisodes.First();
        }

        protected override bool Match(EarningEvent expectedEvent, EarningEvent actualEvent)
        {
            if (expectedEvent.GetType() != actualEvent.GetType())
                return false;

            if (expectedEvent.CollectionPeriod.Name != actualEvent.CollectionPeriod.Name ||
                //expectedEvent.EarningYear != actualEvent.EarningYear ||
                expectedEvent.Learner.ReferenceNumber != actualEvent.Learner.ReferenceNumber ||
                //expectedEvent.Learner.Uln != actualEvent.Learner.Uln ||
                expectedEvent.LearningAim.Reference != actualEvent.LearningAim.Reference ||
                expectedEvent.LearningAim.FundingLineType != actualEvent.LearningAim.FundingLineType ||
                expectedEvent.LearningAim.FrameworkCode != actualEvent.LearningAim.FrameworkCode ||
                expectedEvent.LearningAim.PathwayCode != actualEvent.LearningAim.PathwayCode ||
                expectedEvent.LearningAim.ProgrammeType != actualEvent.LearningAim.ProgrammeType ||
                expectedEvent.LearningAim.StandardCode != actualEvent.LearningAim.StandardCode)
                return false;

            if (!MatchOnProgrammeEarnings(expectedEvent as ApprenticeshipContractType2EarningEvent, actualEvent as ApprenticeshipContractType2EarningEvent))
                return false;

            return true;
        }

        private bool MatchOnProgrammeEarnings(ApprenticeshipContractType2EarningEvent expectedEvent, ApprenticeshipContractType2EarningEvent actualEvent)
        {
            if (expectedEvent == null)
                return true;


            var expectedEventOnProgrammeEarnings = expectedEvent.OnProgrammeEarnings ?? new List<OnProgrammeEarning>().AsReadOnly();
            var actualEventOnProgrammeEarnings = actualEvent.OnProgrammeEarnings ?? new List<OnProgrammeEarning>().AsReadOnly();

            foreach (var expectedEarning in expectedEventOnProgrammeEarnings)
            {
                var actualEarning = actualEventOnProgrammeEarnings.FirstOrDefault(a => a.Type == expectedEarning.Type);

                if (actualEarning == null || !MatchEarningPeriods(actualEarning.Periods, expectedEarning.Periods)) 
                    return false;
            }

            var expectedEventIncentiveEarnings = expectedEvent.IncentiveEarnings ?? new List<IncentiveEarning>().AsReadOnly();
            var actualEventIncentiveEarnings = actualEvent.IncentiveEarnings ?? new List<IncentiveEarning>().AsReadOnly();

            foreach (var expectedEarning in expectedEventIncentiveEarnings)
            {
                var actualEarning = actualEventIncentiveEarnings.FirstOrDefault(a => a.Type == expectedEarning.Type);

                if (actualEarning == null || !MatchEarningPeriods(actualEarning.Periods, expectedEarning.Periods)) 
                    return false;
            }

            return true;
        }

        private static bool MatchEarningPeriods(ReadOnlyCollection<EarningPeriod> actualEarningPeriods, ReadOnlyCollection<EarningPeriod> expectedEarningPeriods)
        {
            if (actualEarningPeriods.Count != expectedEarningPeriods.Count)
                return false;

            for (var i = 0; i < expectedEarningPeriods.Count; i++)
            {
                if (expectedEarningPeriods[i].Amount != actualEarningPeriods[i].Amount ||
                    expectedEarningPeriods[i].PriceEpisodeIdentifier != actualEarningPeriods[i].PriceEpisodeIdentifier ||
                    expectedEarningPeriods[i].Period != actualEarningPeriods[i].Period)
                    return false;
            }

            return true;
        }
    }
}