using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class PeriodEndSteps : EndToEndStepsBase
    {
        private SubmissionDataFactory submissionDataFactory;

        public PeriodEndSteps(FeatureContext context) : base(context)
        {
            var submissionDataContext = Scope.Resolve<SubmissionDataContext>();
            submissionDataFactory = new SubmissionDataFactory(submissionDataContext);
        }

        [Given(@"there are submission summaries for (.*)")]
        public void GivenThereAreSubmissionSummariesFor(string collectionPeriod)
        {
            SetCollectionPeriod(collectionPeriod);
            submissionDataFactory.CreateSubmissionSummaryModel(CollectionPeriod, AcademicYear);
        }

        [Given(@"the submission percentage (.*) within tolerance")]
        public async Task GivenTheSubmissionPercentageIsWithinTolerance(string isWithinTolerance)
        {
            if (isWithinTolerance == "is")
            {
                submissionDataFactory.SetPercentageWithInTolerance();
            }
            else
            {
                submissionDataFactory.SetPercentageOutOfDefaultTolerance();
            }

            await submissionDataFactory.SaveModel();
        }

        [When(@"DC request period end submission window validation")]
        public async Task WhenDCRequestPeriodEndSubmissionWindowValidation()
        {
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendPeriodEndTask(AcademicYear, CollectionPeriod, TestSession.JobId, "PeriodEndSubmissionWindowValidation");
        }

        [Then(@"DC job is updated with (.*) status")]
        public async Task ThenDCJobIsUpdatedWithStatus(JobStatus jobStatus)
        {
            await WaitForIt(
                async () =>
            {
                var job = await submissionDataFactory.GetPeriodEndSubmissionWindowValidationJob(TestSession.JobId, CollectionPeriod, AcademicYear);
                return job != null && job.Status == jobStatus;
            }, $"Failed to find validation job with status: {jobStatus}");
        }
    }
}