using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class FM36BreakInLearningBaseSteps : FM36_ILR_Base_Steps
    {
        public FM36BreakInLearningBaseSteps(FeatureContext context) : base(context)
        {
        }

        [Given("a Learner has been made redundant")]
        public async Task LearnerMadeRedundant()
        {
            ImportR03Fm36();

            TestSession.FM36Global.UKPRN = TestSession.Provider.Ukprn;
            TestSession.FM36Global.Learners.ForEach(x => x.ULN = TestSession.Learner.Uln);
            TestSession.FM36Global.Learners.ForEach(x => x.LearnRefNumber = TestSession.Learner.LearnRefNumber);

            TestSession.Employers.Clear();
            TestSession.Employers.Add(new Employer { AccountId = TestSession.GenerateId(), Balance = 999999999, IsLevyPayer = true, AccountName = "employer 0" });
            TestSession.Employers.Add(new Employer { AccountId = TestSession.GenerateId(), Balance = 999999999, IsLevyPayer = true, AccountName = "employer 1" });

            foreach (var employer in TestSession.Employers)
            {
                await SaveLevyAccount(employer).ConfigureAwait(false);
            }

            ScriptInApprenticeshipsAndPriceEpisodes(); //This creates apprenticeships and apprenticeship prices episodes to match the apprenticeships in the FM36s and the scenario in the ticket

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await WaitForRequiredPayments(6);

            await EmployerMonthEndHelper.SendLevyMonthEndForEmployers(
                TestSession.GenerateId(),
                TestSession.Employers.Select(x => x.AccountId),
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                MessageSession);

            await WaitForPayments(6);
        }

        [Given("a break in learning has also occurred")]
        [Given("the redundancy and break in learning have been correctly recorded in the ILR")]
        [Given("the delivered learning days before the break are under 75 percent")]
        public void EmptyStep()
        {
            //covered by FM36s imported
            //R03 introduces a redundancy and BIL with the following fields:
            //PriceEpisodeRedStartDate
            //PriceEpisodeRedStatusCode
        }

        [When("the learner is re-employed before the 12 weeks redundancy period is exhausted")]
        public async Task LearnerReEmployed()
        {
            ImportR04Fm36();
            TestSession.FM36Global.UKPRN = TestSession.Provider.Ukprn;
            TestSession.FM36Global.Learners.ForEach(x => x.ULN = TestSession.Learner.Uln);
            TestSession.FM36Global.Learners.ForEach(x => x.LearnRefNumber = TestSession.Learner.LearnRefNumber);

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await WaitForRequiredPayments(2);

            await EmployerMonthEndHelper.SendLevyMonthEndForEmployers(
                TestSession.GenerateId(),
                TestSession.Employers.Select(x => x.AccountId),
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                MessageSession);

            await WaitForPayments(2);
        }

        private void ScriptInApprenticeshipsAndPriceEpisodes()
        {
            var fm36Year = TestSession.FM36Global.Learners.First().PriceEpisodes.First().PriceEpisodeValues.EpisodeStartDate.Value.Year;

            var sql = $@"

            DELETE FROM [Payments2].[ApprenticeshipPriceEpisode] WHERE [ApprenticeshipId] IN (5001, 5002)
            DELETE FROM [Payments2].[Apprenticeship] WHERE [Id] IN (5001, 5002)

INSERT INTO [Payments2].[Apprenticeship]
           ([Id]
           ,[AccountId]
           ,[AgreementId]
           ,[AgreedOnDate]
           ,[Uln]
           ,[Ukprn]
           ,[EstimatedStartDate]
           ,[EstimatedEndDate]
           ,[Priority]
           ,[StandardCode]
           ,[ProgrammeType]
           ,[FrameworkCode]
           ,[PathwayCode]
           ,[LegalEntityName]
           ,[TransferSendingEmployerAccountId]
           ,[StopDate]
           ,[Status]
           ,[IsLevyPayer]
           ,[CreationDate]
           ,[ApprenticeshipEmployerType])
     VALUES
           (5001,	{TestSession.Employers[0].AccountId},	'515SIE',	'01/08/{fm36Year + 2}',	{TestSession.Learner.Uln},	{TestSession.Provider.Ukprn},	'01/08/{fm36Year}',	'01/08/{fm36Year + 5}',	0,	17,	25,	0,	0,	'ZURICH TEST',	NULL,	'{fm36Year + 2}-11-05',	3,	1,	CURRENT_TIMESTAMP,	1)


INSERT INTO [Payments2].[Apprenticeship]
           ([Id]
           ,[AccountId]
           ,[AgreementId]
           ,[AgreedOnDate]
           ,[Uln]
           ,[Ukprn]
           ,[EstimatedStartDate]
           ,[EstimatedEndDate]
           ,[Priority]
           ,[StandardCode]
           ,[ProgrammeType]
           ,[FrameworkCode]
           ,[PathwayCode]
           ,[LegalEntityName]
           ,[TransferSendingEmployerAccountId]
           ,[StopDate]
           ,[Status]
           ,[IsLevyPayer]
           ,[CreationDate]
           ,[ApprenticeshipEmployerType])
     VALUES
           (5002,	{TestSession.Employers[1].AccountId},	'515SIE',	'01/08/{fm36Year + 2}',	{TestSession.Learner.Uln},	{TestSession.Provider.Ukprn},	'11/07/{fm36Year + 2}',	'01/08/{fm36Year + 5}',	0,	17,	25,	0,	0,	'ZURICH TEST',	NULL,	NULL,	1,	1,	CURRENT_TIMESTAMP,	1)


INSERT INTO [Payments2].[ApprenticeshipPriceEpisode]
           ([ApprenticeshipId]
           ,[StartDate]
           ,[EndDate]
           ,[Cost]
           ,[Removed]
           ,[CreationDate])
     VALUES
           (5001,	'01/08/{fm36Year}',	NULL,	15000,	0           ,CURRENT_TIMESTAMP)


INSERT INTO [Payments2].[ApprenticeshipPriceEpisode]
           ([ApprenticeshipId]
           ,[StartDate]
           ,[EndDate]
           ,[Cost]
           ,[Removed]
           ,[CreationDate])
     VALUES
           (5002,	'11/07/{fm36Year + 2}',	NULL,	5000,	0           ,CURRENT_TIMESTAMP)

";

            var command = DataContext.Database.GetDbConnection().CreateCommand();
            DataContext.Database.OpenConnection();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        private void ImportR03Fm36() { GetFm36LearnerForCollectionPeriod("R03/current academic year"); }
        private void ImportR04Fm36() { GetFm36LearnerForCollectionPeriod("R04/current academic year"); }
    }
}