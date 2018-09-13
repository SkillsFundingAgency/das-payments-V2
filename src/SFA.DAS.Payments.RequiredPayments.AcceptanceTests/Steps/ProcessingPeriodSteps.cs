using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class ProcessingPeriodSteps
    {
        private readonly ScenarioContext context;

        public ProcessingPeriodSteps(ScenarioContext context)
        {
            this.context = context;
        }

        [Given(@"the current processing period is (.*)")]
        public void GivenTheCurrentProcessingPeriodIs(short period)
        {
            context["ProcessingPeriod"] = period;
        }

    }
}