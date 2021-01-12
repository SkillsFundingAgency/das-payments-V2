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
    [Scope(Feature = "PV2-2268-Break-in-Learning-after-redundancy-with-delivered-days-under-75-percent-then-re-employed-before-12-weeks-is-up")]
    // ReSharper disable once InconsistentNaming
    public class PV2_2268_Steps : FM36_ILR_Base_Steps
    {
        public PV2_2268_Steps(FeatureContext context) : base(context)
        {
        }

        private const string PriceEpisodeIdentifierR04 = "PE-2268-R04";

        [Given("a Learner has been made redundant")]
        public async Task LearnerMadeRedundant()
        {
            ImportR03Fm36();
            
            TestSession.FM36Global.UKPRN = TestSession.Provider.Ukprn;
            TestSession.FM36Global.Learners.ForEach(x => x.ULN = TestSession.Learner.Uln);
            TestSession.FM36Global.Learners.ForEach(x => x.LearnRefNumber = TestSession.Learner.LearnRefNumber);

            TestSession.Employers.Clear();
            TestSession.Employers.Add(new Employer { AccountId = TestSession.GenerateId(), Balance = 999999999, IsLevyPayer = true, AccountName = "employer 0"});
            TestSession.Employers.Add(new Employer { AccountId = TestSession.GenerateId(), Balance = 999999999, IsLevyPayer = true, AccountName = "employer 1"});

            foreach (var employer in TestSession.Employers)
            {
                await SaveLevyAccount(employer).ConfigureAwait(false);
            }

            ScriptInApprenticeshipData();

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

        [Given("a break in learning has also occurred")]
        [Given("the redundancy and break in learning have been correctly recorded in the ILR")]
        [Given("the delivered learning days before the break are under 75 percent")]
        public void EmptyStep()
        {
            //covered by FM36s imported in first step
        }

        [Then("the learner must be funded from the employer levy funds")]
        public async Task LearnerFundedFromLevy()
        {
            await WaitForIt(() => HasCorrectlyFundedPaymentFromLevyForR04(),
                "Failed to find learning payment funded from Levy for R04");
        }

        private bool HasCorrectlyFundedPaymentFromLevyForR04()
        {
            return FundingSourcePaymentEventsHelper
                .FundingSourcePaymentsReceivedForLearner(PriceEpisodeIdentifierR04, short.Parse(TestSession.FM36Global.Year), TestSession)
                .Any(x =>
                    x.FundingSourceType == FundingSourceType.Levy
                    && x.DeliveryPeriod == 4
                    && x.TransactionType == TransactionType.Learning);
        }

        private void ImportR03Fm36() { GetFm36LearnerForCollectionPeriod("R03/current academic year"); }
        private void ImportR04Fm36() { GetFm36LearnerForCollectionPeriod("R04/current academic year"); }

        private void ScriptInApprenticeshipData()
        {
            var sql = $@"USE [SFA.DAS.Payments.Database]

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
           (5001,	{TestSession.Employers[0].AccountId},	'515SIE',	'01/08/2020',	{TestSession.Learner.Uln},	{TestSession.Provider.Ukprn},	'01/08/2018',	'01/08/2023',	0,	17,	25,	0,	0,	'ZURICH TEST',	NULL,	'2020-11-05',	3,	1,	CURRENT_TIMESTAMP,	1)


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
           (5002,	{TestSession.Employers[1].AccountId},	'515SIE',	'01/08/2020',	{TestSession.Learner.Uln},	{TestSession.Provider.Ukprn},	'11/07/2020',	'01/08/2023',	0,	17,	25,	0,	0,	'ZURICH TEST',	NULL,	NULL,	1,	1,	CURRENT_TIMESTAMP,	1)


INSERT INTO [Payments2].[ApprenticeshipPriceEpisode]
           ([ApprenticeshipId]
           ,[StartDate]
           ,[EndDate]
           ,[Cost]
           ,[Removed]
           ,[CreationDate])
     VALUES
           (5001,	'01/08/2018',	NULL,	15000,	0           ,CURRENT_TIMESTAMP)


INSERT INTO [Payments2].[ApprenticeshipPriceEpisode]
           ([ApprenticeshipId]
           ,[StartDate]
           ,[EndDate]
           ,[Cost]
           ,[Removed]
           ,[CreationDate])
     VALUES
           (5002,	'11/07/2020',	NULL,	5000,	0           ,CURRENT_TIMESTAMP)

";

            var command = DataContext.Database.GetDbConnection().CreateCommand();
            DataContext.Database.OpenConnection();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

       
    }
}