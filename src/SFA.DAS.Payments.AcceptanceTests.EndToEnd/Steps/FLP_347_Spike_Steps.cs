using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "FLP-347-Spike")]
    public class FLP_347_Spike_Steps : FM36_ILR_Base_Steps
    {
        public FLP_347_Spike_Steps(FeatureContext context) : base(context) { }

        private const string PriceEpisodeIdentifier = "FPE-347";
        private const string CommitmentIdentifier = "FA-347";
        private CollectionPeriod CollectionPeriod = new CollectionPeriod
        {
            AcademicYear = 2324,
            Period = 1
        };

        private long AccountId = 21063; //max levy balance employer

        [Given("A CalculatedRequiredLevyAmount event is published for a basic levy learner")]
        public async Task FireEventForLevyLearner()
        {
            try
            {
                var calculatedRequiredLevyAmount = CalculatedRequiredLevyAmountBuilder
                    .BuildDefaultEvent(PriceEpisodeIdentifier, AccountId, CollectionPeriod)
                    .AddSubmissionDate();

                var options = new PublishOptions();
                await MessageSession.Publish(calculatedRequiredLevyAmount, options);
                //Thread.Sleep(5000);
                //todo wait for this to complete somehow?
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [When("Month end is ran for the learner's provider")]
        public async Task RunMonthEnd()
        {
            var monthEndJobId = TestSession.GenerateId();
            await EmployerMonthEndHelper.SendLevyMonthEndForEmployers(
                monthEndJobId,
                new List<long>{ AccountId },
                CollectionPeriod.AcademicYear,
                CollectionPeriod.Period,
                MessageSession);
        }

        [Then("The correct payments should be generated for that learner")]
        public async Task VerifyPayments()
        {

        }

        
    }

    public static class CalculatedRequiredLevyAmountBuilder
    {
        public static CalculatedRequiredLevyAmount BuildDefaultEvent(string priceEpisodeIdentifier, long? accountId, CollectionPeriod collectionPeriod)
        {
            return new CalculatedRequiredLevyAmount
            {
                Priority = 1,
                AgreementId = "123456789",
                AgreedOnDate = new DateTime(2022, 8, 1),
                SfaContributionPercentage = 0.95m,
                OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                EarningEventId = Guid.Empty,
                ClawbackSourcePaymentEventId = null,
                PriceEpisodeIdentifier = priceEpisodeIdentifier,
                AmountDue = 1000.0m,
                DeliveryPeriod = 1,
                AccountId = accountId,
                TransferSenderAccountId = null,
                ContractType = ContractType.Act1,
                StartDate = new DateTime(2022, 8, 1),
                PlannedEndDate = new DateTime(2022, 8, 1).AddDays(365),
                ActualEndDate = new DateTime(2022, 8, 1).AddDays(365),
                CompletionStatus = 0,
                CompletionAmount = 3000.0m,
                InstalmentAmount = 1000.0m,
                NumberOfInstalments = 12,
                LearningStartDate = new DateTime(2022, 8, 1),
                ApprenticeshipId = 987654321,
                ApprenticeshipPriceEpisodeId = 1234567890,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                ReportingAimFundingLineType = "16-18 Apprenticeship (From May 2017) Levy Contract",
                LearningAimSequenceNumber = 1,
                JobId = 123456,
                EventTime = DateTimeOffset.Now,
                EventId = Guid.NewGuid(),
                Ukprn = 123456781,
                Learner = new Model.Core.Learner { ReferenceNumber = "2349874", Uln = 2304897 },
                LearningAim = new LearningAim
                {
                    Reference = "AIM123",
                    ProgrammeType = 1,
                    StandardCode = 1234,
                    FrameworkCode = 5678,
                    PathwayCode = 9012,
                    FundingLineType = "16-18 Apprenticeship (From May 2017) Levy Contract",
                    SequenceNumber = 1,
                    StartDate = new DateTime(2022, 8, 1)
                },
                //IlrSubmissionDateTime = new DateTime(2022, 8, 1), //
                //IlrFileName = "Run 2", //
                CollectionPeriod = new CollectionPeriod { AcademicYear = collectionPeriod.AcademicYear, Period = collectionPeriod.Period }
            };
        }

        public static CalculatedRequiredLevyAmount AddSubmissionDate(this CalculatedRequiredLevyAmount calculatedRequiredLevyAmount)
        {
            calculatedRequiredLevyAmount.IlrSubmissionDateTime = calculatedRequiredLevyAmount.StartDate;
            return calculatedRequiredLevyAmount;
        }
    }
}