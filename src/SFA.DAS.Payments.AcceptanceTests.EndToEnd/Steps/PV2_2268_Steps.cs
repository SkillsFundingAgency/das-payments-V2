using System.Collections.Generic;
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
    [Scope(Feature = "PV2-2268-spike")]
    // ReSharper disable once InconsistentNaming
    public class PV2_2268_Steps : FM36_ILR_Base_Steps
    {
        public PV2_2268_Steps(FeatureContext context) : base(context)
        {
        }

        private const string PriceEpisodeIdentifier = "PE-2268";
        private const string CommitmentIdentifier = "A-2268";

        [Given("run spike")]
        public async Task RunSpike()
        {
            ImportR03Fm36();
            //TestSession.Provider.Ukprn = TestSession.FM36Global.UKPRN; //hardcoded ukprn
            TestSession.FM36Global.UKPRN = TestSession.Provider.Ukprn; //anonymised ukprn
            TestSession.FM36Global.Learners.ForEach(x => x.ULN = TestSession.Learner.Uln); //one learner nuke all ulns to be the same from test session

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

            ImportR04Fm36();
            TestSession.FM36Global.UKPRN = TestSession.Provider.Ukprn; //anonymised ukprn
            TestSession.FM36Global.Learners.ForEach(x => x.ULN = TestSession.Learner.Uln); //one learner nuke all ulns to be the same from test session

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