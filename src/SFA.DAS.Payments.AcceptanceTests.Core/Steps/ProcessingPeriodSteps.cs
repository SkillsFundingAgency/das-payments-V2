using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Steps
{
    [Binding]
    public class ProcessingPeriodSteps: StepsBase
    {
        private readonly ScenarioContext context;

        public ProcessingPeriodSteps(ScenarioContext context): base(context) { }

        [Given(@"the current processing period is (.*)")]
        public void GivenTheCurrentProcessingPeriodIs(short period)
        {
            context["ProcessingPeriod"] = period;
        }

        [Given(@"the collection year is (.*)")]
        public void GivenTheCollectionYearIs(string collectionYear)
        {
            CollectionYear = collectionYear;
        }
    }
}