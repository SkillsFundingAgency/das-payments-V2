using System.Collections.Generic;
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
                context["RequiredPaymentsContractType2PayableEarnings"] as
                    IEnumerable<RequiredPayment>;

            var learnRefNumber = context["LearnRefNumber"].ToString();
            var uln = (long) context["Uln"];
            var ukprn = (long) context["Ukprn"];
            var generatedLearnRefNumber = context["GeneratedLearnRefNumber"].ToString();


        }

    }
}
