using System;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Payments.Messages.Core.Events;
using Earning = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.Earning;
using FunctionalSkillEarning = SFA.DAS.Payments.Model.Core.Incentives.FunctionalSkillEarning;
using Learner = SFA.DAS.Payments.Model.Core.Learner;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class EarningEventMatcher : BaseMatcher<EarningEvent>
    {
        private readonly TestSession testSession;
        private readonly CalendarPeriod collectionPeriod;
        private readonly IList<Earning> earningSpecs;
        private readonly IList<FM36Learner> learnerSpecs;

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

            foreach (var learnerId in learnerIds)
            {
                var learnerSpec = testSession.GetLearner(learnerId);
                var fm36learner = learnerSpecs.Single(l => l.LearnRefNumber == learnerSpec.LearnRefNumber);
                var learner = new Learner
                {
                    ReferenceNumber = learnerSpec.LearnRefNumber,
                    Uln = learnerSpec.Uln
                };

                if (learnerSpec.Aims.Count == 0)
                    learnerSpec.Aims.Add(new Aim
                    {
                        LearnerId = learnerSpec.LearnerIdentifier,
                        ProgrammeType = learnerSpec.Course.ProgrammeType,
                        FrameworkCode = learnerSpec.Course.FrameworkCode,
                        PathwayCode = learnerSpec.Course.PathwayCode,
                        StandardCode = learnerSpec.Course.StandardCode,
                        FundingLineType = learnerSpec.Course.FundingLineType,
                        AimReference = learnerSpec.Course.LearnAimRef
                    });

                foreach (var aimSpec in learnerSpec.Aims.Where(a => AimPeriodMatcher.IsStartDateValidForCollectionPeriod(a.StartDate, collectionPeriod,
                    a.PlannedDurationAsTimespan, a.ActualDurationAsTimespan, a.CompletionStatus, a.AimReference)))
                {
                    var learningAim = new LearningAim
                    {
                        ProgrammeType = aimSpec.ProgrammeType,
                        FrameworkCode = aimSpec.FrameworkCode,
                        PathwayCode = aimSpec.PathwayCode,
                        StandardCode = aimSpec.StandardCode,
                        FundingLineType = aimSpec.FundingLineType,
                        Reference = aimSpec.AimReference
                    };

                    var aimEarningSpecs = earningSpecs.Where(e => e.LearnerId == learnerId && e.AimSequenceNumber.GetValueOrDefault(aimSpec.AimSequenceNumber) == aimSpec.AimSequenceNumber).ToList();
                    var fullListOfTransactionTypes = aimEarningSpecs.SelectMany(p => p.Values.Keys).Distinct().ToList();
                    var onProgEarnings = fullListOfTransactionTypes.Where(EnumHelper.IsOnProgType).ToList();
                    var functionalSkillEarnings = fullListOfTransactionTypes.Where(EnumHelper.IsFunctionalSkillType).ToList();
                    var incentiveEarnings = fullListOfTransactionTypes.Where(EnumHelper.IsIncentiveType).ToList();

                    if (aimSpec.AimReference == "ZPROG001" && onProgEarnings.Any())
                    {
                        var onProgEarning = new ApprenticeshipContractType2EarningEvent
                        {
                            CollectionPeriod = collectionPeriod,
                            Ukprn = testSession.Ukprn,
                            OnProgrammeEarnings = onProgEarnings.Select(tt => new OnProgrammeEarning
                            {
                                Type = (OnProgrammeEarningType)(int)tt,
                                Periods = aimEarningSpecs.Select(e => new EarningPeriod
                                {
                                    Amount = e.Values[tt],
                                    Period = e.DeliveryCalendarPeriod.Period,
                                    PriceEpisodeIdentifier = FindPriceEpisodeIdentifier(e.Values[tt], e, fm36learner, tt)
                                }).ToList().AsReadOnly()
                            }).ToList().AsReadOnly(),
                            JobId = testSession.JobId,
                            Learner = learner,
                            LearningAim = learningAim
                        };
                        result.Add(onProgEarning);
                    }

                    if (aimSpec.AimReference != "ZPROG001" && functionalSkillEarnings.Any())
                    {
                        var functionalSkillEarning = new FunctionalSkillEarningsEvent
                        {
                            CollectionPeriod = collectionPeriod,
                            Ukprn = testSession.Ukprn,
                            Earnings = functionalSkillEarnings.Select(tt => new FunctionalSkillEarning
                            {
                                Type = (FunctionalSkillType)(int)tt,
                                Periods = aimEarningSpecs.Select(e => new EarningPeriod
                                {
                                    Amount = e.Values[tt],
                                    Period = e.DeliveryCalendarPeriod.Period,
                                    PriceEpisodeIdentifier = FindPriceEpisodeIdentifier(e.Values[tt], e, fm36learner, tt)
                                }).ToList().AsReadOnly()
                            }).ToList().AsReadOnly(),
                            JobId = testSession.JobId,
                            Learner = learner,
                            LearningAim = learningAim
                        };
                        result.Add(functionalSkillEarning);
                    }

                    if (incentiveEarnings.Any())
                    {
                        var incentiveEarning = new ApprenticeshipContractType2EarningEvent
                        {
                            CollectionPeriod = collectionPeriod,
                            Ukprn = testSession.Ukprn,
                            IncentiveEarnings = incentiveEarnings.Select(tt => new IncentiveEarning
                            {
                                Type = (IncentiveEarningType)(int)tt,
                                Periods = aimEarningSpecs.Select(e => new EarningPeriod
                                {
                                    Amount = e.Values[tt],
                                    Period = e.DeliveryCalendarPeriod.Period,
                                    PriceEpisodeIdentifier = FindPriceEpisodeIdentifier(e.Values[tt], e, fm36learner, tt)
                                }).ToList().AsReadOnly()
                            }).ToList().AsReadOnly(),
                            JobId = testSession.JobId,
                            Learner = learner,
                            LearningAim = learningAim
                        };

                        result.Add(incentiveEarning);
                    }
                }
            }

            return result;
        }

        private string FindPriceEpisodeIdentifier(decimal value, Earning earning, FM36Learner fm36Learner, TransactionType transactionType)
        {
            // null for 0 values
            if (value == 0)
                return null;

            // it could be specified in earnings table
            if (earning.PriceEpisodeIdentifier != null)
                return earning.PriceEpisodeIdentifier;

            // find first price episode with non-zero value for a period
            var period = earning.DeliveryCalendarPeriod.Period;
            return fm36Learner.PriceEpisodes.SingleOrDefault(pe => pe.PriceEpisodePeriodisedValues.Any(pepv =>
                pepv.GetValue(period).GetValueOrDefault(0) != 0 &&
                pepv.AttributeName == transactionType.ToAttributeName()))?.PriceEpisodeIdentifier;
        }

        protected override bool Match(EarningEvent expectedEvent, EarningEvent actualEvent)
        {
            if (expectedEvent.GetType() != actualEvent.GetType())
                return false;

            if (expectedEvent.CollectionPeriod.Name != actualEvent.CollectionPeriod.Name ||
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
                    expectedEarningPeriods[i].Period != actualEarningPeriods[i].Period)
                    return false;

                if (expectedEarningPeriods[i].Amount != 0 &&
                    expectedEarningPeriods[i].PriceEpisodeIdentifier != actualEarningPeriods[i].PriceEpisodeIdentifier)
                    return false;
            }

            return true;
        }
    }
}