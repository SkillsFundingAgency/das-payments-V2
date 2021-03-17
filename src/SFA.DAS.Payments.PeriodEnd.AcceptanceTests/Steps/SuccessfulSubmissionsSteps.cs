using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.PeriodEnd.AcceptanceTests.Infrastructure;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PeriodEnd.AcceptanceTests.Steps
{
    [Binding]
    public class SuccessfulSubmissionsSteps : StepsBase
    {
        protected FunctionWrapper Functions { get; private set; }
        private static readonly HttpClient Client = new HttpClient();
        private static readonly short AcademicYear = 2021;
        private static readonly byte CollectionPeriod = 15;

        public SuccessfulSubmissionsSteps(ScenarioContext context) : base(context)
        { }

        [BeforeScenario]
        public void Setup()
        {
            TestSession.RegenerateUkprn();
            Functions = new FunctionWrapper(Client);
            Console.WriteLine($"Generated new UKPRN: {TestSession.Ukprn}");
        }
        
        [Given(@"the provider makes a (failed|successful) submission")]
        [When(@"the provider makes a (failed|successful) submission")]
        public async Task GivenTheProviderMakesASuccessfulSubmission(string submissionStatus)
        {
            // "failed" or "successful"
            if (submissionStatus.Equals("failed", StringComparison.OrdinalIgnoreCase))
                await MessageSession.Publish<SubmissionJobFailed>(m =>
                {
                    m.Ukprn = TestSession.Ukprn;
                });
            else
            {
                Container.Resolve<TestPaymentsDataContext>().CreateSuccessfulSubmissionJob(TestSession.Ukprn, AcademicYear, CollectionPeriod);
                await MessageSession.Publish<SubmissionJobSucceeded>(m =>
                {
                    m.Ukprn = TestSession.Ukprn;
                });
            }
        }

        [When(@"there is a change to the apprenticeship details for one of the provider's learners")]
        [Given(@"there is a change to the apprenticeship details for one of the provider's learners")]
        public async Task WhenThereIsAChangeToTheApprenticeshipDetailsForOneOfTheProviderSLearners()
        {
            await MessageSession.Publish<ApprenticeshipUpdated>(m =>
            {
                m.Ukprn = TestSession.Ukprn;
            });
        }

        [Then(@"the provider should (not )?appear in the results")]
        public async Task ThenTheProviderShouldNotAppearInTheResults(string not)
        {
            // "not " or ""
            if (not.Equals("not ", StringComparison.OrdinalIgnoreCase))
            {
                await WaitForIt(async () =>
                {
                    var result = await Functions.SuccessfulSubmissions(AcademicYear, CollectionPeriod);
                    return result.SuccessfulSubmissionJobs.All(x => x.Ukprn != TestSession.Ukprn);
                }, "Provider found in successful submissions");

                await WaitForUnexpected(() =>
                {
                    var result = Functions.SuccessfulSubmissions(AcademicYear, CollectionPeriod).Result;
                    return (result.SuccessfulSubmissionJobs.All(x => x.Ukprn != TestSession.Ukprn), "Provider found in successful submissions");
                }, "Provider found in successful submissions");
            }
            else
            {
                await WaitForIt(async () =>
                {
                    var result = await Functions.SuccessfulSubmissions(AcademicYear, CollectionPeriod);
                    return result.SuccessfulSubmissionJobs.Any(x => x.Ukprn == TestSession.Ukprn);
                }, "Provider did not appear in the successful submissions output");
            }
        }
    }
}
