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
    public class ApprenticeshipContractType2EarningEventMatcher: BaseEarningEventMatcher
    {
        
        public ApprenticeshipContractType2EarningEventMatcher(IList<Earning> earningSpecs, TestSession testSession, CollectionPeriod collectionPeriod, IList<FM36Learner> learnerSpecs)
            : base(earningSpecs, testSession, collectionPeriod, learnerSpecs)
        {
        }

        protected override ApprenticeshipContractTypeEarningsEvent CreateOnProgEarning(List<TransactionType> onProgEarnings, List<Earning> aimEarningSpecs, FM36Learner fm36Learner, Learner learner, LearningAim learningAim)
        {
            var onProgEvent = new ApprenticeshipContractType2EarningEvent();
            return MapToApprenticeshipContractTypeEarningsEventWithoutIncentives(onProgEvent,onProgEarnings, aimEarningSpecs, fm36Learner, learner, learningAim);
        }
        protected override ApprenticeshipContractTypeEarningsEvent CreateIncentiveEarning(List<TransactionType> incentiveEarnings, List<Earning> aimEarningSpecs, FM36Learner fm36Learner, Learner learner, LearningAim learningAim)
        {
            var incentiveEvent = new ApprenticeshipContractType2EarningEvent();
            return MapToApprenticeshipContractTypeEarningsEventWithIncentives(incentiveEvent, incentiveEarnings, aimEarningSpecs, fm36Learner, learner, learningAim);
        }

        protected override bool ValidateOnProgEarnings(EarningEvent expectedEvent, EarningEvent actualEvent)
        {
            return !MatchOnProgrammeEarnings(expectedEvent as ApprenticeshipContractType2EarningEvent, actualEvent as ApprenticeshipContractType2EarningEvent);
        }


    }
}
