using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Application;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Data;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Handlers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class FundingSourceOutputSteps : StepsBase
    {
        private readonly LearnRefNumberGenerator generator;

        public FundingSourceOutputSteps(ScenarioContext context, LearnRefNumberGenerator generator): base(context)
        {
            this.generator = generator;
        }

        [Then(@"the funding source component will generate the following contract type (.*) coinvested payments:")]
        public void ThenTheFundingSourceComponentWillGenerateTheFollowingContractTypeCoinvestedPayments(short contractType, Table table)
        {
            WaitForIt(() =>
            {
                var results = SfaCoInvestedFundingSourceHandler.ReceivedEvents;

                if (results == null || !results.Any())
                {
                    return false;
                }

                var payments = table.CreateSet<FundingSourcePayment>();

                return payments.All(x => results.Any(resultEvent => x.Ukprn == resultEvent.Ukprn
                                                                           && generator.Generate(x.Ukprn, x.LearnRefNumber)
                                                                               .ToString() ==
                                                                           resultEvent.Learner.ReferenceNumber
                                                                           && x.TransactionType == (short)resultEvent.OnProgrammeEarningType
                                                                           && x.Amount == resultEvent.AmountDue));
            }, "Failed to find all the coinvested funding source payment events");
        }

    }
}