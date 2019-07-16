using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.PeriodEnd.AcceptanceTests.Handlers;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PeriodEnd.AcceptanceTests
{
    [Binding]
    public class PeriodEndSteps : StepsBase
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

        [When(@"the period end service is notified the the period end has stopped")]
        public async Task WhenThePeriodEndServiceIsNotifiedTheThePeriodEndHasStopped()
        {
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendPeriodEndTask(AcademicYear, CollectionPeriod, TestSession.JobId, "PeriodEndStop").ConfigureAwait(false);
        }

        [When(@"the period end service is notified the the period end is running")]
        public async Task WhenThePeriodEndServiceIsNotifiedTheThePeriodEndIsRunning()
        {
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendPeriodEndTask(AcademicYear, CollectionPeriod, TestSession.JobId, "PeriodEndRun").ConfigureAwait(false);
        }


        [Then(@"the period end service should publish a period end started event")]
        public async Task ThenThePeriodEndServiceShouldPublishAPeriodEndStartedEvent()
        {
            await WaitForIt(() =>
                {
                    return PeriodEndStartedEventHandler.ReceivedEvents.Any(ev => ev.JobId == TestSession.JobId);
                }, $"Failed to find the period end started event for job : { TestSession.JobId}");
        }

        [Then(@"the period end service should publish a period end running event")]
        public async Task ThenThePeriodEndServiceShouldPublishAPeriodEndRunningEvent()
        {
            await WaitForIt(() =>
            {
                return PeriodEndRunningEventHandler.ReceivedEvents.Any(ev => ev.JobId == TestSession.JobId);
            }, $"Failed to find the period end running event for job : { TestSession.JobId}");
        }


        [Then(@"the period end service should publish a period end stopped event")]
        public async Task ThenThePeriodEndServiceShouldPublishAPeriodEndStoppedEvent()
        {
            await WaitForIt(() =>
            {
                return PeriodEndStoppedEventHandler.ReceivedEvents.Any(ev => ev.JobId == TestSession.JobId);
            }, $"Failed to find the period end stopped event for job : { TestSession.JobId}");
        }

    }
}