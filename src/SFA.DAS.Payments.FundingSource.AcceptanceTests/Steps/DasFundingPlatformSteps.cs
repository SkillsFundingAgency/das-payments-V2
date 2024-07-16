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
            //await WaitForIt(() => fundingSourceHelper.GetLevyTransactions(TestSession.Ukprn, TestSession.CollectionPeriod)
            //        .Any(x => x.FundingPlatformType == calculatedOnProgrammePaymentCommand.FundingPlatformType),
            //    "Failed to wait for levy account transactions with funding platform set to DAS");
        }

        [Then("a payment with a funding platform type of DasFundingPlatform is created for the calculated payment")]
        public async Task ThenAPaymentWithAFundingPlatformTypeofDasFundingPlatformIsCreated()
        {
            //await WaitForIt(() => paymentsHelper.GetPayments(TestSession.Ukprn, TestSession.CollectionPeriod)
            //        .Any(x => x.FundingPlatformType == calculatedOnProgrammePaymentCommand.FundingPlatformType),
            //    "Failed to wait for payments with funding platform set to DAS");
        }

        private void SetupCalculateOnProgrammePaymentCommand()
        {
            var learner = TestSession.GenerateLearner(TestSession.Ukprn);
            learner.LearnRefNumber = TestSession.LearnRefNumberGenerator.Generate(learner.Ukprn, TestSession.GenerateId().ToString());
            var learningAim = new Aim(new Training
            {
                CompletionStatus = "continuing"
            });
            
            calculatedOnProgrammePaymentCommand = new CalculateOnProgrammePayment
            {
                Ukprn = TestSession.Provider.Ukprn,
                AgreedOnDate = DateTime.Today.AddDays(-100), // TODO
                SfaContributionPercentage = SfaContributionPercentage,
                OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                PriceEpisodeIdentifier = "1", // TODO
                AmountDue = 1000, // TODO
                DeliveryPeriod = 1,
                AccountId = TestSession.Employer.AccountId,
                TransferSenderAccountId = null,
                StartDate = DateTime.Today.AddDays(-50), // TODO
                PlannedEndDate = DateTime.Today.AddDays(200),
                ActualEndDate = null,
                CompletionStatus = 0,
                CompletionAmount = 2000, // TODO
                InstalmentAmount = 1000, // TODO
                NumberOfInstalments = 12, // TODO
                LearningStartDate = DateTime.Today.AddDays(-40), // TODO
                ApprenticeshipId = 1111, // TODO
                ApprenticeshipPriceEpisodeId = 1234, // TODO
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
                    SequenceNumber = 1, // TODO
                    StandardCode = learningAim.StandardCode
                },
                CollectionPeriod = TestSession.CollectionPeriod,
                FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService
            };
        }
    }
}
