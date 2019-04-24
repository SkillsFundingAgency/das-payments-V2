using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using Earning = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.Earning;
using Learner = SFA.DAS.Payments.Model.Core.Learner;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class PayableEarningEventMatcher : BaseMatcher<PayableEarningEvent>
    {
        private readonly Provider provider;
        private readonly IList<Earning> earningSpecs;
        private readonly TestSession testSession;
        private readonly CollectionPeriod collectionPeriod;
        private readonly IList<FM36Learner> learnerSpecs;

        public PayableEarningEventMatcher(Provider provider,IList<Earning> earningSpecs, TestSession testSession, CollectionPeriod collectionPeriod, IList<FM36Learner> learnerSpecs)
        {
            this.provider = provider;
            this.earningSpecs = earningSpecs;
            this.testSession = testSession;
            this.collectionPeriod = collectionPeriod;
            this.learnerSpecs = learnerSpecs;
        }

        protected override IList<PayableEarningEvent> GetActualEvents()
        {
            return PayableEarningEventHandler.ReceivedEvents.Where(e => e.JobId == provider.JobId
                                                                        && e.CollectionPeriod.Period == collectionPeriod.Period
                                                                        && e.CollectionYear == collectionPeriod.AcademicYear
                                                                        && e.Ukprn == provider.Ukprn).ToList();
        }

        protected override IList<PayableEarningEvent> GetExpectedEvents()
        {
            var result = new List<PayableEarningEvent>();
            var learnerIds = earningSpecs.Select(e => e.LearnerId).Distinct().ToList();

            foreach (var learnerId in learnerIds)
            {
                var learnerSpec = testSession.GetLearner(provider.Ukprn, learnerId);
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

                foreach (var aimSpec in learnerSpec.Aims.Where(a => AimPeriodMatcher.IsStartDateValidForCollectionPeriod(a.StartDate, collectionPeriod,
                    a.PlannedDurationAsTimespan, a.ActualDurationAsTimespan, a.CompletionStatus, a.AimReference, a.PlannedDuration, a.ActualDuration)))
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
                    var incentiveEarnings = fullListOfTransactionTypes.Where(EnumHelper.IsIncentiveType).ToList();

                    if (aimSpec.AimReference == "ZPROG001" && onProgEarnings.Any())
                    {
                        var onProgEarning = new PayableEarningEvent
                        {
                            CollectionPeriod = collectionPeriod,
                            Ukprn = provider.Ukprn,
                            OnProgrammeEarnings = onProgEarnings.Select(tt => new OnProgrammeEarning
                            {
                                Type = (OnProgrammeEarningType)(int)tt,
                                Periods = aimEarningSpecs.Select(e => new EarningPeriod
                                {
                                    Amount = e.Values[tt],
                                    Period = e.DeliveryCalendarPeriod,
                                    PriceEpisodeIdentifier = FindPriceEpisodeIdentifier(e.Values[tt], e, fm36Learner, tt)
                                }).ToList().AsReadOnly()
                            }).ToList(),
                            JobId = provider.JobId,
                            Learner = learner,
                            LearningAim = learningAim
                        };
                        result.Add(onProgEarning);
                    }

                    if (incentiveEarnings.Any())
                    {
                        var incentiveEarning = new PayableEarningEvent
                        {
                            CollectionPeriod = collectionPeriod,
                            Ukprn = provider.Ukprn,
                            IncentiveEarnings = incentiveEarnings.Select(tt => new IncentiveEarning
                            {
                                Type = (IncentiveEarningType)(int)tt,
                                Periods = aimEarningSpecs.Select(e => new EarningPeriod
                                {
                                    Amount = e.Values[tt],
                                    Period = e.DeliveryCalendarPeriod,
                                    PriceEpisodeIdentifier = FindPriceEpisodeIdentifier(e.Values[tt], e, fm36Learner, tt)
                                }).ToList().AsReadOnly()
                            }).ToList(),
                            JobId = provider.JobId,
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
            var period = earning.DeliveryCalendarPeriod;
            return fm36Learner.PriceEpisodes
                .SingleOrDefault(pe => pe.PriceEpisodePeriodisedValues
                    .Any(pepv => pepv.GetValue(period).GetValueOrDefault(0) != 0 &&
                                 pepv.AttributeName == transactionType.ToAttributeName()))?
                .PriceEpisodeIdentifier;
        }

        protected override bool Match(PayableEarningEvent expectedEvent, PayableEarningEvent actualEvent)
        {
            if (expectedEvent.GetType() != actualEvent.GetType())
                return false;

            if (expectedEvent.CollectionPeriod.Period != actualEvent.CollectionPeriod.Period ||
                expectedEvent.CollectionPeriod.AcademicYear != actualEvent.CollectionPeriod.AcademicYear ||
                expectedEvent.Learner.ReferenceNumber != actualEvent.Learner.ReferenceNumber ||
                expectedEvent.LearningAim.Reference != actualEvent.LearningAim.Reference ||
                expectedEvent.LearningAim.FundingLineType != actualEvent.LearningAim.FundingLineType ||
                expectedEvent.LearningAim.FrameworkCode != actualEvent.LearningAim.FrameworkCode ||
                expectedEvent.LearningAim.PathwayCode != actualEvent.LearningAim.PathwayCode ||
                expectedEvent.LearningAim.ProgrammeType != actualEvent.LearningAim.ProgrammeType ||
                expectedEvent.LearningAim.StandardCode != actualEvent.LearningAim.StandardCode 
                //TODO: should every period be checked as these properties have now moved to earning period
                //|| expectedEvent.Priority != actualEvent.Priority ||
                //expectedEvent.AccountId != actualEvent.AccountId ||
                //expectedEvent.CommitmentId != actualEvent.CommitmentId ||
                //expectedEvent.CommitmentVersion != actualEvent.CommitmentVersion
                )
                return false;

            return true;
        }
    }
}