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
        public byte CollectionPeriod { get; set; }
        public short AcademicYear { get; set; }

        public SharedSteps()
        {
            config = new TestConfiguration();
            dataFactory = new TestDataFactory(new SubmissionDataContext(config.PaymentsConnectionString));
        }

        [Given(@"SubmissionSumary Exists for CollectionPeriod (.*) and AcademicYear (.*)")]
        public void GivenSubmissionSumaryExistsForCollectionPeriodAndAcademicYear(byte collectionPeriod, short academicYear)
        {
            this.CollectionPeriod = collectionPeriod;
            this.AcademicYear = academicYear;

            dataFactory.CreateSubmissionSummaryModel(collectionPeriod, academicYear);
        }


        [Given(@"SubmissionSumary Does Not Exists for CollectionPeriod (.*) and AcademicYear (.*)")]
        public async Task GivenSubmissionSumaryDoesNotExistsForCollectionPeriodAndAcademicYear(byte collectionPeriod, short academicYear)
        {
            this.CollectionPeriod = collectionPeriod;
            this.AcademicYear = academicYear;

            await dataFactory.RemoveSubmissionSummaryModel(collectionPeriod, academicYear);
        }

        [Given(@"Submission Percentage (\w*\s*\w*) within  Tolerances")]
        public void GivenSubmissionPercentageIsNotWithInTolerances(string isWithInTolerance)
        {
            if (isWithInTolerance == "Is Not")
            {
                dataFactory.SetPercentageOutOfDefaultTolerance();
            }
            else
            {
                dataFactory.SetPercentageWithInTolerance();
            }
        }

        [Given(@"CollectionPeriodTolerances (.*) configured")]
        public async Task GivenCollectionPeriodTolerancesAreConfiguredIsForCollectionPeriodAndAcademicYear(string isTolerancesConfigured)
        {
            if (isTolerancesConfigured == "NOT")
            {
                await dataFactory.RemoveCollectionPeriodToleranceModel(CollectionPeriod, AcademicYear);
            }
            else
            {
                await dataFactory.CreateCollectionPeriodToleranceModel(CollectionPeriod, AcademicYear, 99, 100);
            }
        }

        [When(@"ValidateSubmissionWindow function is called")]
        public async Task WhenValidateSubmissionWindowFunctionIsCalled()
        {
            await dataFactory.SaveModel();

            httpResponseMessage = await new HttpClient().GetAsync($"{config.ValidateSubmissionWindowFunctionUrl}?jobid={123}&collectionPeriod={CollectionPeriod}&academicYear={AcademicYear}");
        }

        [Then(@"HttpStatus code (.*) with Json result is returned")]
        public async Task ThenHttpStatusCodeWithJsonResultIsReturned(string httpStatusCode)
        {
            httpResponseMessage.StatusCode.ToString("D").Should().Be(httpStatusCode);

            var response = await httpResponseMessage.Content.ReadAsStringAsync();
            var submissionSummary = JsonConvert.DeserializeObject<SubmissionsSummaryModel>(response);
            submissionSummary.Should().NotBeNull();
            submissionSummary.IsWithinTolerance.Should().BeTrue();
        }

        [Then(@"HttpStatus code (.*) without Json result is returned")]
        public void ThenHttpStatusCodewithoutJsonResultIsReturned(string httpStatusCode)
        {
            httpResponseMessage.StatusCode.ToString("D").Should().Be(httpStatusCode);
            
            httpResponseMessage.IsSuccessStatusCode.Should().BeFalse();
        }

        [Then(@"The IsWithinTolerance is (.*) in database")]
        public async Task ThenIsWithinToleranceSavedToDatabase(string isSavedToDatabase)
        {
            var data = await dataFactory.GetSubmissionsSummaries(CollectionPeriod, AcademicYear);

            data.Any().Should().BeTrue();

            if (isSavedToDatabase == "False")
            {
                data.Single().IsWithinTolerance.Should().BeFalse();
            }
            else
            {
                data.Single().IsWithinTolerance.Should().BeTrue();
            }

            await ClearData(data);
        }

        [Then(@"SubmissionsSumary is NOT Saved to database")]
        public async Task ThenSubmissionsSumaryNotSavedToDatabase()
        {
            var data = await dataFactory.GetSubmissionsSummaries(CollectionPeriod, AcademicYear);

            data.Any().Should().BeFalse();

            await ClearData(data);
        }

        private async Task ClearData(List<SubmissionsSummaryModel> data)
        {
            await dataFactory.ClearData(data, CollectionPeriod, AcademicYear);
        }
    }
}
