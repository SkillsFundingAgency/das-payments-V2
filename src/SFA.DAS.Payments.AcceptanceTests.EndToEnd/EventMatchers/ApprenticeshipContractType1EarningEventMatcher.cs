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
            var onProgEvent = new ApprenticeshipContractType1EarningEvent();
            return MapToApprenticeshipContractTypeEarningsEventWithoutIncentives(onProgEvent, onProgEarnings, aimEarningSpecs, fm36Learner, learner, learningAim);

        }

        protected override ApprenticeshipContractTypeEarningsEvent CreateIncentiveEarning(List<TransactionType> incentiveEarnings, List<Earning> aimEarningSpecs, FM36Learner fm36Learner, Learner learner, LearningAim learningAim)
        {
            var incentiveEvent = new ApprenticeshipContractType1EarningEvent();
            return MapToApprenticeshipContractTypeEarningsEventWithIncentives(incentiveEvent, incentiveEarnings, aimEarningSpecs, fm36Learner, learner, learningAim);
        }

        protected override bool ValidateOnProgEarnings(EarningEvent expectedEvent, EarningEvent actualEvent)
        {
            return !MatchOnProgrammeEarnings(expectedEvent as ApprenticeshipContractType1EarningEvent, actualEvent as ApprenticeshipContractType1EarningEvent);
        }


    }
}
