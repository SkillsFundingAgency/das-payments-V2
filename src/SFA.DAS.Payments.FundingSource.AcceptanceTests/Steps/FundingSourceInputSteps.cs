using System.Collections.Generic;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class FundingSourceInputSteps
    {
        [When(@"MASH is received")]
        public void WhenMashIsReceived()
        {
            var context = ScenarioContext.Current;

            var payableEarnings =
                context["RequiredPaymentsContractType2PayableEarnings"] as
                    IEnumerable<RequiredPayment>;


        }

    }
}
