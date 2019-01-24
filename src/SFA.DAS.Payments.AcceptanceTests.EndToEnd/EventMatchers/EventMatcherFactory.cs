using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public static class EventMatcherFactory
    {
        public static BaseEarningEventMatcher BuildEarningEventMatcher(
            List<Price> currentPriceEpisodes,
            List<Training> currentIlr,
            IList<Earning> earningSpecs,
            TestSession testSession,
            CollectionPeriod collectionPeriod,
            IList<FM36Learner> learnerSpecs)
        {
            ContractType contractType;

            if (currentIlr == null || !currentIlr.Any())
            {
                if (currentPriceEpisodes == null) throw  new Exception("No valid current Price Episodes found");

                contractType = currentPriceEpisodes.Last().ContractType;
            }
            else
            {
                contractType = currentIlr.Last().ContractType;
            }

            switch (contractType)
            {
                case ContractType.Act1:
                    return new ApprenticeshipContractType1EarningEventMatcher(earningSpecs, testSession, collectionPeriod, learnerSpecs);

                case ContractType.Act2:
                    return new ApprenticeshipContractType2EarningEventMatcher(earningSpecs, testSession, collectionPeriod, learnerSpecs);

                default:
                    throw new InvalidOperationException("Cannot create the EarningEventMatcher invalid contract type ");
            }

        }
    }
}
