using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using Earning = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.Earning;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class ApprenticeshipContractType1EarningEventMatcher: BaseEarningEventMatcher
    {
        private readonly IList<Earning> earningSpecs;
        private readonly TestSession testSession;
        private readonly CollectionPeriod collectionPeriod;
        private readonly IList<FM36Learner> learnerSpecs;

        public ApprenticeshipContractType1EarningEventMatcher(IList<Earning> earningSpecs, TestSession testSession, CollectionPeriod collectionPeriod, IList<FM36Learner> learnerSpecs)
            : base(earningSpecs, testSession, collectionPeriod, learnerSpecs)
        {
            this.earningSpecs = earningSpecs;
            this.testSession = testSession;
            this.collectionPeriod = collectionPeriod;
            this.learnerSpecs = learnerSpecs;
        }

        protected override ApprenticeshipContractTypeEarningsEvent CreateOnProgEarning(List<TransactionType> onProgEarnings, List<Earning> aimEarningSpecs, FM36Learner fm36Learner, Learner learner, LearningAim learningAim)
        {
            return new ApprenticeshipContractType1EarningEvent
            {
                CollectionPeriod = collectionPeriod,
                Ukprn = testSession.Ukprn,
                OnProgrammeEarnings = onProgEarnings.Select(tt => new OnProgrammeEarning
                {
                    Type = (OnProgrammeEarningType)(int)tt,
                    Periods = aimEarningSpecs.Select(e => new EarningPeriod
                    {
                        Amount = e.Values[tt],
                        Period = (byte)e.DeliveryCalendarPeriod,
                        PriceEpisodeIdentifier = FindPriceEpisodeIdentifier(e.Values[tt], e, fm36Learner, tt)
                    }).ToList().AsReadOnly()
                }).ToList().AsReadOnly(),
                JobId = testSession.JobId,
                Learner = learner,
                LearningAim = learningAim
            };
        }

        protected override ApprenticeshipContractTypeEarningsEvent CreateIncentiveEarning(List<TransactionType> incentiveEarnings, List<Earning> aimEarningSpecs, FM36Learner fm36Learner, Learner learner, LearningAim learningAim)
        {
            return new ApprenticeshipContractType1EarningEvent
            {
                CollectionPeriod = collectionPeriod,
                Ukprn = testSession.Ukprn,
                IncentiveEarnings = incentiveEarnings.Select(tt => new IncentiveEarning
                {
                    Type = (IncentiveEarningType)(int)tt,
                    Periods = aimEarningSpecs.Select(e => new EarningPeriod
                    {
                        Amount = e.Values[tt],
                        Period = (byte)e.DeliveryCalendarPeriod,
                        PriceEpisodeIdentifier = FindPriceEpisodeIdentifier(e.Values[tt], e, fm36Learner, tt)
                    }).ToList().AsReadOnly()
                }).ToList().AsReadOnly(),
                JobId = testSession.JobId,
                Learner = learner,
                LearningAim = learningAim
            };
        }

        protected override bool ValidateOnProgEarnings(EarningEvent expectedEvent, EarningEvent actualEvent)
        {
            return !MatchOnProgrammeEarnings(expectedEvent as ApprenticeshipContractType1EarningEvent, actualEvent as ApprenticeshipContractType1EarningEvent);
        }


    }
}
