using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-2083-Test-Harness")]
    // ReSharper disable once InconsistentNaming
    public class PV2_2083_Steps : FM36_ILR_Base_Steps
    {
        public PV2_2083_Steps(FeatureContext context) : base(context)
        {
        }

        private const string PriceEpisodeIdentifier = "PE-2083";
        private const string CommitmentIdentifierDLOCK07 = "A-2083";
        private const string CommitmentIdentifierDLOCK09 = "B-2083";

        [Given("fire test harness")]
        public async Task FireTestHarness()
        {
            ImportR12Fm36();

            await SetUpMatchingCommitment();
            CreateDataLocks();

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);
        }

        private void ImportR12Fm36() { GetFm36LearnerForCollectionPeriod("R12/last academic year"); }

        private void CreateDataLocks()
        {
            //todo here we need to set up the commitments to create the data lock scenario we are after for each commitment
            var context = Scope.Resolve<TestPaymentsDataContext>();

            var apprenticeshipDLOCK09 =
                context.Apprenticeship.Include(a => a.ApprenticeshipPriceEpisodes).Single(x => x.Id == TestSession.Apprenticeships[CommitmentIdentifierDLOCK09].Id);
            apprenticeshipDLOCK09.ApprenticeshipPriceEpisodes.First().StartDate = apprenticeshipDLOCK09.ApprenticeshipPriceEpisodes.First().StartDate.AddDays(3);
            apprenticeshipDLOCK09.EstimatedStartDate = new DateTime(2018, 09, 01); //dlock 9 start date?
            apprenticeshipDLOCK09.StopDate = new DateTime(2019, 06, 01);
            apprenticeshipDLOCK09.Status = ApprenticeshipStatus.Stopped;
            apprenticeshipDLOCK09.ApprenticeshipPriceEpisodes.First().StartDate = new DateTime(2018, 09, 01);

            var apprenticeshipDLOCK07 =
                context.Apprenticeship.Include(a => a.ApprenticeshipPriceEpisodes).Single(x => x.Id == TestSession.Apprenticeships[CommitmentIdentifierDLOCK07].Id);
            apprenticeshipDLOCK07.ApprenticeshipPriceEpisodes.First().Cost -= 1000; //dlock 7 price mismatch
            

            context.SaveChanges();
        }

        private async Task SetUpMatchingCommitment() { await SetupTestCommitmentData(CommitmentIdentifierDLOCK07, PriceEpisodeIdentifier, CommitmentIdentifierDLOCK09, PriceEpisodeIdentifier); }
    }
}