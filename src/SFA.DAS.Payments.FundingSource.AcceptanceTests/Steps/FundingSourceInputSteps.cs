using System.Collections.Generic;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class FundingSourceInputSteps
    {
        private readonly ScenarioContext context;

        public FundingSourceInputSteps(ScenarioContext context)
        {
            this.context = context;
        }

        [When(@"a payable earning event is received")]
        public void WhenAPayableEarningEventIsReceived()
        {
            var payableEarnings =
                context.Get<IEnumerable<RequiredPayment>>();

            var learner = context.Get<Learner>();

            
        }

    }
}
