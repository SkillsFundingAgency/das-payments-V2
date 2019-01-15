using System;
using SFA.DAS.Payments.Tests.Core.Builders;
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
            CollectionYear = new CollectionPeriodBuilder().WithDate(DateTime.Today).Build().AcademicYear;;
        }
    }
}