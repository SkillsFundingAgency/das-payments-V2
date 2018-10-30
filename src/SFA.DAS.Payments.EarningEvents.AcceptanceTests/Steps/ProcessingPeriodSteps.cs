using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    [Binding]
    public class ProcessingPeriodSteps: EarningEventsStepsBase
    {
        public ProcessingPeriodSteps(ScenarioContext context): base(context)
        {
        }

        [Given(@"the current processing period is (.*)")]
        public void GivenTheCurrentProcessingPeriodIs(byte period)
        {
            CollectionPeriod = period;
        }

        [Given(@"the payments are for the current collection year")]
        public void GivenThePaymentsAreForTheCurrentCollectionYear()
        {
            var year = DateTime.Today.Year - 2000;
            CollectionYear = DateTime.Today.Month < 9 ? $"{year - 1}{year}" : $"{year}{year + 1}";
        }
    }
}