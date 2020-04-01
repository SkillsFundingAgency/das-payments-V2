using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.DasHandlers;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Data;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Handlers;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.Tests.Core.Builders;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Steps
{
    [Binding]
    public class MonthEndCompletionPaymentSteps : ProviderPaymentsStepsBase
    {
        [BeforeTestRun(Order = 2)]
        public static void AddDcConfig()
        {
            DcHelper.AddDcConfig(Builder);
        }

        public List<PaymentModel> PaymentModels { get => Get<List<PaymentModel>>(); set => Set(value); }
        public List<RecordedPayment> Payments { get => Get<List<RecordedPayment>>(); set => Set(value); }

        public MonthEndCompletionPaymentSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }


        [Given(@"the collection period is (.*)")]
        public void GivenTheCollectionPeriodIsRCurrentAcademicYear(string p0)
        {
            var collectionPeriod = new CollectionPeriodBuilder().WithSpecDate(p0).Build();

            CollectionPeriod = collectionPeriod.Period;
            AcademicYear = collectionPeriod.AcademicYear;
        }


        [Given(@"the funding source service generates the following contract type payments")]
        public async Task GivenTheFundingSourceServiceGeneratesTheFollowingContractTypePayments(Table payments)
        {

            var context = Container.Resolve<IPaymentsDataContext>();

            await CleanUpPreviousData(context);

            var specPayments = payments.CreateSet<RecordedPayment>().ToList();
            PaymentModels = CreatePayments(specPayments);
            context.Payment.AddRange(PaymentModels);
            await context.SaveChangesAsync();
        }


        private static async Task CleanUpPreviousData(IPaymentsDataContext context)
        {
            context.Payment.RemoveRange(context.Payment.Where(p =>
                p.LearnerReferenceNumber == nameof(MonthEndCompletionPaymentSteps)));
            await context.SaveChangesAsync();
        }

        [Then(@"DAS approvals service should be notified of payments for learners with completion payments")]
        public async Task ThenDASApprovalsServiceShouldBeNotifiedOfPaymentsForLearnersWithCompletionPayments()
        {
            var completionPayments = PaymentModels
                .Where(p =>
                p.TransactionType == TransactionType.Completion &&
                p.ContractType == ContractType.Act1 &&
                p.CollectionPeriod.AcademicYear == AcademicYear &&
                p.CollectionPeriod.Period == CollectionPeriod)
                .ToList();

            await WaitForIt(() =>
            {
                return RecordedAct1CompletionPaymentEventHandler
                           .ReceivedEvents
                           .Count(p =>
                               p.CollectionPeriod.AcademicYear == AcademicYear &&
                               p.CollectionPeriod.Period == CollectionPeriod &&
                               p.ContractType == ContractType.Act1 &&
                               p.TransactionType == TransactionType.Completion)
                    == completionPayments.Count;
            }, $"Failed to find all the provider payment events. Found '{ProviderPaymentEventHandler.ReceivedEvents.Count}' events ");
        }
        
        [When(@"month end stop event is received")]
        public async Task WhenMonthEndStopEventIsReceived()
        {
            var periodEndEvent = new PeriodEndStoppedEvent
            {
                CollectionPeriod = new CollectionPeriod { Period = CollectionPeriod, AcademicYear = AcademicYear }
            };

            await MessageSession.Send(periodEndEvent).ConfigureAwait(false);
        }

        private List<PaymentModel> CreatePayments(List<RecordedPayment> payments)
        {
            var returnPayments = new List<PaymentModel>();
            foreach (var recordedPayment in payments)
            {
                var payment = new PaymentModel
                {
                    CollectionPeriod = recordedPayment.ParsedCollectionPeriod,
                    ContractType = recordedPayment.ContractType,
                    TransactionType = EnumHelper.ToTransactionType(recordedPayment.TransactionType),
                    Amount = recordedPayment.Amount,
                    DeliveryPeriod = 7,
                    Ukprn = 1000001,
                    JobId = 100000,
                    SfaContributionPercentage = 1m,
                    PriceEpisodeIdentifier = "pe-1",
                    FundingSource = FundingSourceType.CoInvestedEmployer,
                    LearningAimPathwayCode = 2,
                    LearnerReferenceNumber = nameof(MonthEndCompletionPaymentSteps),
                    LearningAimReference = "Aim Ref",
                    LearningAimStandardCode = 51,
                    IlrSubmissionDateTime = DateTime.UtcNow,
                    EventId = Guid.NewGuid(),
                    LearningAimFundingLineType = "Funding line type",
                    LearnerUln = 123123123,
                    LearningAimFrameworkCode = 1,
                    LearningAimProgrammeType = 1,
                    AccountId = 100001,
                    TransferSenderAccountId = 100000,
                    StartDate = DateTime.UtcNow,
                    PlannedEndDate = DateTime.UtcNow,
                    ActualEndDate = DateTime.UtcNow,
                    CompletionStatus = 1,
                    CompletionAmount = 100M,
                    InstalmentAmount = 200M,
                    NumberOfInstalments = 12,
                    ReportingAimFundingLineType = "Funding line type",
                    ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                    ApprenticeshipId = 100,
                    ApprenticeshipPriceEpisodeId = 1,
                    LearningStartDate = DateTime.UtcNow.AddMonths(-1)
                };
                returnPayments.Add(payment);
            }

            return returnPayments;
        }
    }
}