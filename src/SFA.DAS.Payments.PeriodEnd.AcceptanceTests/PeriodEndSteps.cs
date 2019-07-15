using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PeriodEnd.AcceptanceTests
{
    [Binding]
    public class PeriodEndSteps: StepsBase
    {
        public PeriodEndSteps(ScenarioContext context) : base(context)
        {

        }

        [Given(@"the payments are for the current collection year")]
        public void GivenThePaymentsAreForTheCurrentCollectionYear()
        {
            SetCurrentCollectionYear();
        }

        [Given(@"the current collection period is R(.*)")]
        [Given(@"the current processing period is (.*)")]
        public void GivenTheCurrentProcessingPeriodIs(byte period)
        {
            CollectionPeriod = period;
        }

        [When(@"the period end service is notified the the period end has started")]
        public async Task WhenThePeriodEndServiceIsNotifiedTheThePeriodEndHasStarted()
        {
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendPeriodEndTask(AcademicYear, CollectionPeriod, TestSession.JobId, "PeriodEndStart").ConfigureAwait(false);
        }

        [Then(@"the period end service should publish a period end started event")]
        public void ThenThePeriodEndServiceShouldPublishAPeriodEndStartedEvent()
        {
            ScenarioContext.Current.Pending();
        }

    }
}