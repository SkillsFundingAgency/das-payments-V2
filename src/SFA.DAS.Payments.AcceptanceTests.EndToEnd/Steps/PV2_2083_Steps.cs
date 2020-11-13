using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-2083-Data-Locks-prevented-from-appearing-on-the-Data-Match-Report")]
    // ReSharper disable once InconsistentNaming
    public class PV2_2083_Steps : FM36_ILR_Base_Steps
    {
        public PV2_2083_Steps(FeatureContext context) : base(context)
        {
        }

        private const string PriceEpisodeIdentifier = "PE-2083";
        private const string FirstCommitmentIdentifier = "A-2083";
        private const string SecondCommitmentIdentifier = "B-2083";

        [Given("two potential matching apprenticeships")]
        [Given("one which fails start date validation but who's start date is later than the second")]
        [Given("a second one which passes start date validation but is stopped")]
        public void EmptyIlrSetupStep()
        {
            //NOTE: This is handled by the FM36 we import
        }

        [When("the ILR is submitted")]
        public async Task TheIlrIsSubmitted()
        {
            ImportR12Fm36();

            await SetUpMatchingCommitment();
            ConfigureApprenticeshipsAccordingToScenario();

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);
        }

        [Then("A DLock 10 is correctly generated")]
        public async Task ADlock10IsCorrectlyGenerated()
        {
            await WaitForIt(
                () => HasDataLock10(),
                "Failed to find a DLock 10");
        }

        private void ImportR12Fm36() { GetFm36LearnerForCollectionPeriod("R12/last academic year"); }

        private void ConfigureApprenticeshipsAccordingToScenario()
        {
            var context = Scope.Resolve<TestPaymentsDataContext>();

            var firstApprenticeship = context.Apprenticeship
                .Include(a => a.ApprenticeshipPriceEpisodes)
                .Single(x => x.Id == TestSession.Apprenticeships[SecondCommitmentIdentifier].Id);

            firstApprenticeship.ApprenticeshipPriceEpisodes.First().StartDate = new DateTime(2018, 09, 01);
            firstApprenticeship.EstimatedStartDate = new DateTime(2018, 09, 01);
            firstApprenticeship.StopDate = new DateTime(2019, 06, 01);
            firstApprenticeship.Status = ApprenticeshipStatus.Stopped;

            var secondApprenticeship = context.Apprenticeship
                .Include(a => a.ApprenticeshipPriceEpisodes)
                .Single(x => x.Id == TestSession.Apprenticeships[FirstCommitmentIdentifier].Id);

            secondApprenticeship.ApprenticeshipPriceEpisodes.First().StartDate = new DateTime(2019, 07, 01);
            secondApprenticeship.EstimatedStartDate = new DateTime(2019, 07, 01);
            secondApprenticeship.Status = ApprenticeshipStatus.Active;

            context.SaveChanges();
        }

        private async Task SetUpMatchingCommitment() { await SetupTestCommitmentData(FirstCommitmentIdentifier, PriceEpisodeIdentifier, SecondCommitmentIdentifier, PriceEpisodeIdentifier); }

        private bool HasDataLock10()
        {
            return EarningEventsHelper
                .GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisodeAndDeliveryPeriod(PriceEpisodeIdentifier, 1920, TestSession, 1)
                .Any(x => x == DataLockErrorCode.DLOCK_10);
        }
    }
}