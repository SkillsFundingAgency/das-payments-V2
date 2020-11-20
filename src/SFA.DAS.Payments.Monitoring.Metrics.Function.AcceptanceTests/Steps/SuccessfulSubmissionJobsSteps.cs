using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.Payments.Monitoring.Metrics.Function.AcceptanceTests.Data;
using SFA.DAS.Payments.Monitoring.Metrics.Function.AcceptanceTests.Infrastructure;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function.AcceptanceTests.Steps
{
    class ScenarioContext : IDisposable
    {
        public SubmissionJobsTestDataBuilder TestDataFactory { get; set; }
        public HttpClient FunctionClient { get; set; }
        public SubmissionJobs Result { get; set; }

        public void Dispose()
        {
            FunctionClient?.Dispose();
        }
    }

    [Binding]
    class SuccessfulSubmissionJobsSteps
    {
        private readonly ScenarioContext scenarioContext;

        private static short academicYear = 2122;
        private static byte collectionPeriod = 15;

        public SuccessfulSubmissionJobsSteps(ScenarioContext context)
        {
            var config = new TestConfiguration();
            var dataContext = new TestSubmissionJobsDataContext(config.PaymentsConnectionString);
            scenarioContext = context; 
            scenarioContext.TestDataFactory = new SubmissionJobsTestDataBuilder(dataContext, academicYear, collectionPeriod++);
            var client = new HttpClient();
            client.BaseAddress = config.SuccessfulSubmissionsFunctionUri;
            scenarioContext.FunctionClient = client;
        }

        [BeforeScenario]
        [AfterScenario]
        public async Task ResetPeriod()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(250));
            scenarioContext.TestDataFactory.ClearDatabase();
        }
        
        [Given(@"there is a submission from a (.*) dc job")]
        public void GivenThereIsASubmissionFromAFailedDcJob(string status)
        {
            if (status.Equals("failed", StringComparison.OrdinalIgnoreCase))
                scenarioContext.TestDataFactory.WithFailedDcJob();
            else
                scenarioContext.TestDataFactory.WithSuccessfulDcJob();
        }

        [Given(@"the job (.*) on das")]
        public void GivenTheJobWasSuccessfullyProcessedOnDas(string status)
        {
            if (status.Equals("failed processing", StringComparison.OrdinalIgnoreCase))
                scenarioContext.TestDataFactory.WithFailedDasJob();
            else
                scenarioContext.TestDataFactory.WithSuccessfulDasJob();
        }

        [When(@"calling the function")]
        public async Task WhenCallingTheFunction()
        {
            scenarioContext.TestDataFactory.AddToDatabase();

            var httpResponseMessage = await scenarioContext.FunctionClient
                .GetAsync($"?collectionPeriod={scenarioContext.TestDataFactory.CollectionPeriod}" +
                          $"&academicYear={scenarioContext.TestDataFactory.AcademicYear}");
            var responseAsString = await httpResponseMessage.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SubmissionJobs>(responseAsString);

            scenarioContext.Result = result;
        }

        [Then(@"the results are empty")]
        public void ThenTheResultsAreEmpty()
        {
            scenarioContext.Result.SuccessfulSubmissionJobs.Should()
                .NotContain(x => x.Ukprn == scenarioContext.TestDataFactory.TestUkprn);
        }

        [Then(@"there is a single instance of the test ukprn")]
        public void ThenThereIsASingleInstanceOfTheTestUkprn()
        {
            scenarioContext.Result.SuccessfulSubmissionJobs.Should()
                .ContainSingle(x => x.Ukprn == scenarioContext.TestDataFactory.TestUkprn);
        }

        [Given(@"there have been (\d*) (.*) submissions for the period")]
        public void GivenThereHaveBeenSubmissionsForThePeriod(int number, string status)
        {
            if (status.Equals("failed", StringComparison.OrdinalIgnoreCase))
            {
                for (int i = 0; i < number; i++)
                {
                    SubmitFailedSubmission();
                }
            }
            else
            {
                for (int i = 0; i < number; i++)
                {
                    SubmitSuccessfulSubmission();
                }
            }
        }

        private void SubmitFailedSubmission()
        {
            scenarioContext.TestDataFactory.WithFailedDasJob().AddToDatabase();
        }

        private void SubmitSuccessfulSubmission()
        {
            scenarioContext.TestDataFactory.WithSuccessfulDasJob().WithSuccessfulDcJob().AddToDatabase();
        }
    }
}
