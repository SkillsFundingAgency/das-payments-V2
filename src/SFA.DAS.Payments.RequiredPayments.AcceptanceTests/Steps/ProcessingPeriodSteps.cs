using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class ProcessingPeriodSteps
    {
        [Given(@"the current processing period is (.*)")]
        public void GivenTheCurrentProcessingPeriodIs(short period)
        {
            ScenarioContext.Current["ProcessingPeriod"] = period;
        }

    }
}