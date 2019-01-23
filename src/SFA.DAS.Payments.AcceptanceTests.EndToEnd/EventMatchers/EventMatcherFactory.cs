using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public static class EventMatcherFactory
    {
        public static BaseEarningEventMatcher BuildEarningEventMatcher(IList<Earning> earningSpecs, TestSession testSession, CollectionPeriod collectionPeriod, IList<FM36Learner> learnerSpecs)
        {
            var testLearner = testSession.Learner ?? testSession.Learners.FirstOrDefault();
            var contractType = testLearner.Aims.FirstOrDefault().PriceEpisodes.FirstOrDefault().ContractType;

            switch (contractType)
            {
                case ContractType.Act1:
                     return new ApprenticeshipContractType2EarningEventMatcher(earningSpecs, testSession, collectionPeriod, learnerSpecs);

                case ContractType.Act2:
                    return null;

                default:
                    throw new InvalidOperationException($"Cannot create the EarningEventMatcher invalid contract type ");
            }
            
        }
    }
}
