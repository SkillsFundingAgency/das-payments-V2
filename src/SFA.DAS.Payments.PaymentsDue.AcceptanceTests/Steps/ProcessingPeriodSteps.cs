using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class ProcessingPeriodSteps
    {
        private readonly ScenarioContext scenarioContext;

        public ProcessingPeriodSteps(ScenarioContext context)
        {
            scenarioContext = context;
        }

        [Given(@"the current processing period is (.*)")]
        public void GivenTheCurrentProcessingPeriodIs(short period)
        {
            scenarioContext["ProcessingPeriod"] = period;
        }

    }
}