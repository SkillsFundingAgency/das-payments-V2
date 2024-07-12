using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class DasFundingPlatformSteps : StepsBase
    {
        private CalculateOnProgrammePayment calculatedOnProgrammePaymentCommand;

        public DasFundingPlatformSteps(SpecFlowContext context) : base(context)
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
            var fundingSourceHelper = Scope.Resolve<IFundingSourceHelper>();

            await WaitForIt(() => fundingSourceHelper
                    .GetLevyTransactions(TestSession.Provider.Ukprn, TestSession.CollectionPeriod)
                    .All(x => x.FundingPlatformType == calculatedOnProgrammePaymentCommand.FundingPlatformType),
                "Failed to wait for levy account transactions with funding platform set to DAS");
        }

        [Then("a payment with a funding platform type of DasFundingPlatform is created for the calculated payment")]
        public async Task ThenAPaymentWithAFundingPlatformTypeofDasFundingPlatformIsCreated()
        {
            var paymentsHelper = Scope.Resolve<IPaymentsHelper>();

            await WaitForIt(() => paymentsHelper.GetPaymentsCount(TestSession.Provider.Ukprn, TestSession.CollectionPeriod) == 1,
                "Failed to wait for generated payment");

            await WaitForIt(() => paymentsHelper.GetPayments(TestSession.Provider.Ukprn, TestSession.CollectionPeriod)
                .All(x => x.FundingPlatformType == calculatedOnProgrammePaymentCommand.FundingPlatformType), "Failed to wait for payments with funding platform set to DAS");
        }

        private void SetupCalculateOnProgrammePaymentCommand()
        {
            var learner = TestSession.GenerateLearner(TestSession.Ukprn);
            learner.LearnRefNumber = TestSession.LearnRefNumberGenerator.Generate(learner.Ukprn, TestSession.GenerateId().ToString());
            var learningAim = learner.Aims[0];
            
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
