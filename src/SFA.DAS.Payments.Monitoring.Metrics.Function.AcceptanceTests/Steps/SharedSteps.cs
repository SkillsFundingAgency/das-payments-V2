using System.Collections.Generic;
using System.Linq;
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
    [Binding]
    public class SharedSteps
    {
        private readonly TestDataFactory dataFactory;
        private readonly TestConfiguration config;

        private HttpResponseMessage httpResponseMessage;

        public SharedSteps()
        {
            config = new TestConfiguration();
            dataFactory = new TestDataFactory(new SubmissionDataContext(config.PaymentsConnectionString));
        }

        [Given(@"SubmissionSumary Exists for CollectionPeriod (.*) and AcademicYear (.*)")]
        public void GivenSubmissionSumaryExistsForCollectionPeriodAndAcademicYear(byte collectionPeriod, short academicYear)
        {
            dataFactory.CreateSubmissionSummaryModel(collectionPeriod, academicYear);
        }

        [Given(@"SubmissionSumary Does Not Exists for CollectionPeriod (.*) and AcademicYear (.*)")]
        public async Task GivenSubmissionSumaryDoesNotExistsForCollectionPeriodAndAcademicYear(byte collectionPeriod, short academicYear)
        {
            await dataFactory.RemoveSubmissionSummaryModel(collectionPeriod, academicYear);
        }

        [Given(@"Submission Percentage (\w*\s*\w*) within  Tolerances")]
        public void GivenSubmissionPercentageIsNotWithInTolerances(string isWithInTolerance)
        {
            if (isWithInTolerance == "is NOT")
            {
                dataFactory.SetPercentageOutOfDefaultTolerance();
            }
            else
            {
                dataFactory.SetPercentageWithInTolerance();
            }
        }

        [Given(@"CollectionPeriodTolerances (.*) configured for CollectionPeriod (.*) and AcademicYear (.*)")]
        public async Task GivenCollectionPeriodTolerancesAreConfiguredIsForCollectionPeriodAndAcademicYear(string isTolerancesConfigured, byte collectionPeriod, short academicYear)
        {
            if (isTolerancesConfigured == "NOT")
            {
                await dataFactory.RemoveCollectionPeriodToleranceModel(collectionPeriod, academicYear);
            }
            else
            {
                await dataFactory.CreateCollectionPeriodToleranceModel(collectionPeriod, academicYear, 99, 100);
            }
        }

        [When(@"ValidateSubmissionWindow function is called for CollectionPeriod (.*) and AcademicYear (.*)")]
        public async Task WhenValidateSubmissionWindowFunctionIsCalled(byte collectionPeriod, short academicYear)
        {
            await dataFactory.SaveModel();

            httpResponseMessage = await new HttpClient().GetAsync($"{config.ValidateSubmissionWindowFunctionUrl}?jobid={123}&collectionPeriod={collectionPeriod}&academicYear={academicYear}");
        }

        [Then(@"HttpStatus code (.*) with Json result is returned")]
        public async Task ThenHttpStatusCodeWithJsonResultIsReturned(string httpStatusCode)
        {
            httpResponseMessage.StatusCode.ToString("D").Should().Be(httpStatusCode);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var response = await httpResponseMessage.Content.ReadAsStringAsync();
                var submissionSummary = JsonConvert.DeserializeObject<SubmissionsSummaryModel>(response);
                submissionSummary.Should().NotBeNull();
                submissionSummary.IsWithinTolerance.Should().BeTrue();
            }
        }

        [Then(@"The IsWithinTolerance is (.*) in database for CollectionPeriod (.*) and AcademicYear (.*)")]
        public async Task ThenIsWithinToleranceSavedToDatabase(string isSavedToDatabase, byte collectionPeriod, short academicYear)
        {
            var data = await dataFactory.GetSubmissionsSummaries(collectionPeriod, academicYear);

            data.Any().Should().BeTrue();

            if (isSavedToDatabase == "False")
            {
                data.Single().IsWithinTolerance.Should().BeFalse();
            }
            else
            {
                data.Single().IsWithinTolerance.Should().BeTrue();
            }

            await ClearData(data, collectionPeriod, academicYear);
        }

        [Then(@"SubmissionsSumary is NOT Saved to database for CollectionPeriod (.*) and AcademicYear (.*)")]
        public async Task ThenSubmissionsSumaryNotSavedToDatabase(byte collectionPeriod, short academicYear)
        {
            var data = await dataFactory.GetSubmissionsSummaries(collectionPeriod, academicYear);

            data.Any().Should().BeFalse();

            await ClearData(data, collectionPeriod, academicYear);
        }

        private async Task ClearData(List<SubmissionsSummaryModel> data, byte collectionPeriod, short academicYear)
        {
            await dataFactory.ClearData(data, collectionPeriod, academicYear);
        }
    }
}
