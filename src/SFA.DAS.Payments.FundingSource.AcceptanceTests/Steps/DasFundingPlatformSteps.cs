using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using TechTalk.SpecFlow;
using Learner = SFA.DAS.Payments.Model.Core.Learner;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class DasFundingPlatformSteps : StepsBase
    {
        private IPaymentsHelper paymentsHelper;
        private IFundingSourceHelper fundingSourceHelper;
        private CollectionPeriod collectionPeriod;

        private CalculateOnProgrammePayment calculatedOnProgrammePaymentCommand;
        
        [BeforeScenario]
        public void InitialiseNewTestDataContext()
        {
            paymentsHelper = Scope.Resolve<IPaymentsHelper>();
            fundingSourceHelper = Scope.Resolve<IFundingSourceHelper>();
        }

        protected DasFundingPlatformSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [When("a calculate on programme payment command is received")]
        public async Task WhenACalculateOnProgrammePaymentCommandIsReceived()
        {
            SetupCalculateOnProgrammePaymentCommand();
            await MessageSession.Send(calculatedOnProgrammePaymentCommand).ConfigureAwait(false);
        }

        [Then("a funding source levy transaction is created for the calculated payment")]
        public async Task ThenAFundingSourceLevyTransactionIsCreated()
        {
            await WaitForIt(() => fundingSourceHelper.GetLevyTransactions(TestSession.Ukprn, collectionPeriod)
                    .Any(x => x.FundingPlatformType == calculatedOnProgrammePaymentCommand.FundingPlatformType),
                "Failed to wait for levy account transactions with funding platform set to DAS");
        }

        private void SetupCalculateOnProgrammePaymentCommand()
        {
            collectionPeriod = new CollectionPeriod { AcademicYear = 2324, Period = 1 };

            var learner = TestSession.GenerateLearner(TestSession.Ukprn);
            learner.LearnRefNumber = TestSession.LearnRefNumberGenerator.Generate(learner.Ukprn, TestSession.GenerateId().ToString());
            var learningAim = new Aim(new Training
            {
                CompletionStatus = "continuing",
                StartDate = DateTime.Today.AddDays(30).ToShortDateString(),
                FrameworkCode = Convert.ToInt32(TestSession.GenerateId()),
                FundingLineType = "19+ Apprenticeship (From May 2017) Levy Contract",
                PathwayCode = Convert.ToInt32(TestSession.GenerateId()),
                ProgrammeType = Convert.ToInt32(TestSession.GenerateId()),
                AimReference = "ZPROG001",
                StandardCode = Convert.ToInt32(TestSession.GenerateId())
            });
            
            calculatedOnProgrammePaymentCommand = new CalculateOnProgrammePayment
            {
                Ukprn = TestSession.Provider.Ukprn,
                AgreedOnDate = DateTime.Today.AddDays(-100),
                SfaContributionPercentage = SfaContributionPercentage,
                OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                PriceEpisodeIdentifier = "1", 
                AmountDue = 1000, 
                DeliveryPeriod = 1,
                AccountId = TestSession.Employer.AccountId,
                TransferSenderAccountId = null,
                StartDate = DateTime.Today.AddDays(-50), 
                PlannedEndDate = DateTime.Today.AddDays(200),
                ActualEndDate = null,
                CompletionStatus = 0,
                CompletionAmount = 2000, 
                InstalmentAmount = 1000, 
                NumberOfInstalments = 12, 
                LearningStartDate = DateTime.Today.AddDays(-40), 
                ApprenticeshipId = TestSession.GenerateId(),
                ApprenticeshipPriceEpisodeId = TestSession.GenerateId(),
                ApprenticeshipEmployerType = learner.IsLevyLearner
                    ? ApprenticeshipEmployerType.Levy
                    : ApprenticeshipEmployerType.NonLevy,
                EventTime = DateTimeOffset.UtcNow,
                EventId = Guid.NewGuid(),
                Learner = new Learner { ReferenceNumber = learner.LearnRefNumber, Uln = learner.Uln }, 
                LearningAim = new LearningAim
                {
                    StartDate = Convert.ToDateTime(learningAim.StartDate),
                    FrameworkCode = learningAim.FrameworkCode,
                    FundingLineType = learningAim.FundingLineType,
                    PathwayCode = learningAim.PathwayCode,
                    ProgrammeType = learningAim.ProgrammeType,
                    Reference = learningAim.AimReference,
                    SequenceNumber = 1,
                    StandardCode = learningAim.StandardCode
                },
                CollectionPeriod = collectionPeriod,
                FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService
            };
        }
    }
}
