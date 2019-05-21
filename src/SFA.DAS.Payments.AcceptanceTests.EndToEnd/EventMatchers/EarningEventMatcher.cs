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
    public class EarningEventMatcher: BaseMatcher<EarningEvent>
    {
        private readonly TestSession testSession;
        private readonly CollectionPeriod collectionPeriod;
        private readonly Provider provider;
        private readonly List<Price> currentPriceEpisodes;
        private readonly List<Training> currentIlr;
        private readonly IList<Earning> earningSpecs;
        private readonly IList<FM36Learner> learnerSpecs;

        public EarningEventMatcher(Provider provider , List<Price> currentPriceEpisodes,List<Training> currentIlr, IList<Earning> earningSpecs, TestSession testSession, CollectionPeriod collectionPeriod, IList<FM36Learner> learnerSpecs)
        {
            this.provider = provider;
            this.currentPriceEpisodes = currentPriceEpisodes;
            this.currentIlr = currentIlr;
            this.earningSpecs = earningSpecs;
            this.testSession = testSession;
            this.collectionPeriod = collectionPeriod;
            this.learnerSpecs = learnerSpecs;
        }

        protected override IList<EarningEvent> GetActualEvents()
        {
            return EarningEventHandler.ReceivedEvents
                       .Where(e => e.JobId == provider.JobId
                                   && e.CollectionPeriod.Period == collectionPeriod.Period
                                   && e.CollectionYear == collectionPeriod.AcademicYear
                                   && e.Ukprn == provider.Ukprn)
                .ToList();
        }

        protected override IList<EarningEvent> GetExpectedEvents()
        {
            var result = new List<EarningEvent>();
            var learnerIds = earningSpecs.Where(e => e.Ukprn == provider.Ukprn).Select(e => e.LearnerId).Distinct().ToList();

            foreach (var learnerId in learnerIds)
            {
                var learnerSpec = testSession.GetLearner(provider.Ukprn,learnerId);
                var fm36Learner = learnerSpecs.Single(l => l.LearnRefNumber == learnerSpec.LearnRefNumber);
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

                IEnumerable<Aim> currentAims;

                // AimPeriodMatcher doesn't support completed aims from the past, this is a workaround until imminent refactoring
                if (learnerSpec.Aims.Count == 1)
                {
                    currentAims = learnerSpec.Aims;
                }
                else
                {
                    currentAims = learnerSpec.Aims.Where(a => AimPeriodMatcher.IsStartDateValidForCollectionPeriod(a.StartDate, collectionPeriod,
                        a.PlannedDurationAsTimespan, a.ActualDurationAsTimespan, a.CompletionStatus, a.AimReference, a.PlannedDuration, a.ActualDuration));
                }

                foreach (var aimSpec in currentAims)
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
                    var functionalSkillEarnings = fullListOfTransactionTypes.Where(t => EnumHelper.IsFunctionalSkillType(t, aimSpec.IsMainAim)).ToList();
                    var incentiveEarnings = fullListOfTransactionTypes.Where(t => EnumHelper.IsIncentiveType(t, aimSpec.IsMainAim)).ToList();

                    if (aimSpec.IsMainAim && onProgEarnings.Any())
                    {
                        var contractTypeEarningsEvents  = CreateContractTypeEarningsEventEarningEvent(provider.Ukprn);

                        foreach (var onProgEarning in contractTypeEarningsEvents)
                        {
                            onProgEarning.OnProgrammeEarnings = onProgEarnings.Select(tt => new OnProgrammeEarning
                            {
                                Type = (OnProgrammeEarningType)(int)tt,
                                Periods = GetEarningPeriods(aimEarningSpecs, aimSpec, onProgEarning, tt, fm36Learner).AsReadOnly()
                            }).ToList();
                            onProgEarning.JobId = provider.JobId;
                            onProgEarning.Learner = learner;
                            onProgEarning.LearningAim = learningAim;

                            result.Add(onProgEarning);
                        }
                    }

                    if (!aimSpec.IsMainAim && functionalSkillEarnings.Any())
                    {
                        var functionalSkillEarning = new FunctionalSkillEarningsEvent
                        {
                            CollectionPeriod = collectionPeriod,
                            Ukprn = provider.Ukprn,
                            Earnings = functionalSkillEarnings.Select(tt => new FunctionalSkillEarning
                            {
                                Type = (FunctionalSkillType)(int)tt,
                                Periods = aimEarningSpecs.Select(e => new EarningPeriod
                                {
                                    Amount = e.Values[tt],
                                    Period = e.DeliveryCalendarPeriod,
                                    PriceEpisodeIdentifier = FindPriceEpisodeIdentifier(e.Values[tt], e, fm36Learner, tt)
                                }).ToList().AsReadOnly()
                            }).ToList().AsReadOnly(),
                            JobId = provider.JobId,
                            Learner = learner,
                            LearningAim = learningAim
                        };
                        result.Add(functionalSkillEarning);
                    }

                    if (!aimSpec.IsMainAim && incentiveEarnings.Any())
                    {
                        var contractTypeIncentiveEarnings = CreateContractTypeEarningsEventEarningEvent(provider.Ukprn);

                        foreach (var incentiveEarning in contractTypeIncentiveEarnings)
                        {
                            incentiveEarning.IncentiveEarnings = incentiveEarnings.Select(tt => new IncentiveEarning
                            {
                                Type = (IncentiveEarningType)(int)tt,
                                Periods = GetEarningPeriods(aimEarningSpecs, aimSpec, incentiveEarning, tt, fm36Learner).AsReadOnly(),
                            }).ToList();
                            incentiveEarning.JobId = provider.JobId;
                            incentiveEarning.Learner = learner;
                            incentiveEarning.LearningAim = learningAim;

                            result.Add(incentiveEarning);
                        }
                    }
                }
            }

            return result;
        }

        private List<EarningPeriod> GetEarningPeriods(List<Earning> aimEarningSpecs, Aim aimSpec, ApprenticeshipContractTypeEarningsEvent onProgEarning, TransactionType tt, FM36Learner fm36Learner)
        {
            var matchingPriceEpisodeEarningPeriods = aimEarningSpecs
                .Where(e => GetPriceEpisodeContractType(aimSpec.PriceEpisodes, e.PriceEpisodeIdentifier, onProgEarning))
                .Select(e => new EarningPeriod
                {
                    Amount = e.Values[tt],
                    Period = e.DeliveryCalendarPeriod,
                    PriceEpisodeIdentifier = FindPriceEpisodeIdentifier(e.Values[tt], e, fm36Learner, tt)
                }).ToList();

            var nonMatchingPriceEpisodeEarningPeriods = aimEarningSpecs
                .Where(e => !GetPriceEpisodeContractType(aimSpec.PriceEpisodes, e.PriceEpisodeIdentifier, onProgEarning))
                .Select(e => new EarningPeriod
                {
                    Amount = 0M,
                    Period = e.DeliveryCalendarPeriod,
                    PriceEpisodeIdentifier = FindPriceEpisodeIdentifier(e.Values[tt], e, fm36Learner, tt)
                }).ToList();

            matchingPriceEpisodeEarningPeriods.AddRange(nonMatchingPriceEpisodeEarningPeriods);

            return matchingPriceEpisodeEarningPeriods.OrderBy(p => p.Period).ToList();
        }

        private static bool GetPriceEpisodeContractType(List<Price> priceEpisodes, string priceEpisodeIdentifier,
            IEarningEvent onProgEarning)
        {
            var matchingPriceEpisode = priceEpisodes.FirstOrDefault(p =>
                p.PriceEpisodeId == priceEpisodeIdentifier);

            switch (onProgEarning)
            {
                case ApprenticeshipContractType1EarningEvent _:
                    return matchingPriceEpisode?.ContractType == ContractType.Act1;
                case ApprenticeshipContractType2EarningEvent _:
                    return matchingPriceEpisode?.ContractType == ContractType.Act2;
                default:
                    return false;
            }
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
            var period = earning.DeliveryCalendarPeriod;

            earning.PriceEpisodeIdentifier = fm36Learner.PriceEpisodes
                .SingleOrDefault(pe => pe.PriceEpisodePeriodisedValues
                    .Any(pepv => pepv.GetValue(period).GetValueOrDefault(0) != 0 &&
                                 pepv.AttributeName == transactionType.ToAttributeName()))?
                .PriceEpisodeIdentifier;

            return earning.PriceEpisodeIdentifier;
        }

        protected override bool Match(EarningEvent expectedEvent, EarningEvent actualEvent)
        {
            if (expectedEvent.GetType() != actualEvent.GetType())
                return false;

            if (expectedEvent.CollectionPeriod.Period != actualEvent.CollectionPeriod.Period ||
                expectedEvent.CollectionPeriod.AcademicYear != actualEvent.CollectionPeriod.AcademicYear ||
                expectedEvent.Learner.ReferenceNumber != actualEvent.Learner.ReferenceNumber ||
                //expectedEvent.Learner.Uln != actualEvent.Learner.Uln ||
                expectedEvent.LearningAim.Reference != actualEvent.LearningAim.Reference ||
                expectedEvent.LearningAim.FundingLineType != actualEvent.LearningAim.FundingLineType ||
                expectedEvent.LearningAim.FrameworkCode != actualEvent.LearningAim.FrameworkCode ||
                expectedEvent.LearningAim.PathwayCode != actualEvent.LearningAim.PathwayCode ||
                expectedEvent.LearningAim.ProgrammeType != actualEvent.LearningAim.ProgrammeType ||
                expectedEvent.LearningAim.StandardCode != actualEvent.LearningAim.StandardCode)
                return false;

            if (!MatchOnProgrammeEarnings(expectedEvent as ApprenticeshipContractTypeEarningsEvent, actualEvent as ApprenticeshipContractTypeEarningsEvent))
                return false;

            return true;
        }

        private bool MatchOnProgrammeEarnings(ApprenticeshipContractTypeEarningsEvent expectedEvent, ApprenticeshipContractTypeEarningsEvent actualEvent)
        {
            if (expectedEvent == null)
                return true;


            var expectedEventOnProgrammeEarnings = expectedEvent.OnProgrammeEarnings ?? new List<OnProgrammeEarning>();
            var actualEventOnProgrammeEarnings = actualEvent.OnProgrammeEarnings ?? new List<OnProgrammeEarning>();

            foreach (var expectedEarning in expectedEventOnProgrammeEarnings)
            {
                var actualEarning = actualEventOnProgrammeEarnings.FirstOrDefault(a => a.Type == expectedEarning.Type);

                if (actualEarning == null || !MatchEarningPeriods(actualEarning.Periods, expectedEarning.Periods))
                    return false;
            }

            var expectedEventIncentiveEarnings = expectedEvent.IncentiveEarnings ?? new List<IncentiveEarning>();
            var actualEventIncentiveEarnings = actualEvent.IncentiveEarnings ?? new List<IncentiveEarning>();

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

        private List<ApprenticeshipContractTypeEarningsEvent> CreateContractTypeEarningsEventEarningEvent( long ukprn)
        {
            var contractTypes = EnumHelper.GetContractTypes(currentIlr, currentPriceEpisodes);

            var events = new List<ApprenticeshipContractTypeEarningsEvent>();

            contractTypes.ForEach(c =>
            {
                switch (c)
                {
                    case ContractType.Act1:
                        events.Add(new ApprenticeshipContractType1EarningEvent
                        {
                            CollectionPeriod = collectionPeriod,
                            Ukprn = ukprn
                        });
                        break;
                    case ContractType.Act2:
                        events.Add(new ApprenticeshipContractType2EarningEvent
                        {
                            CollectionPeriod = collectionPeriod,
                            Ukprn = ukprn
                        });
                        break;
                    default:
                        throw new InvalidOperationException(
                            "Cannot create the EarningEventMatcher invalid contract type ");
                }
            });

            return events;
        }


    }
}
