using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Data;
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
           var specPayments = payments.CreateSet<RecordedPayment>().ToList();
            var paymentModels = CreatePayments(specPayments);
            var context = Container.Resolve<IPaymentsDataContext>();
            context.Payment.AddRange(paymentModels);
            await context.SaveChangesAsync();
        }

        [Then(@"DAS approvals service should be notified of payments for learners with completion payments")]
        public async Task ThenDASApprovalsServiceShouldBeNotifiedOfPaymentsForLearnersWithCompletionPayments()
        {
           
        }

        [When(@"month end stop event is received")]
        public async Task WhenMonthEndStopEventIsReceived()
        {
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendPeriodEndTask(AcademicYear, CollectionPeriod, TestSession.JobId, "PeriodEndStop").ConfigureAwait(false);
        }
        
        [Then(@"only learners with completion payments should have events generated")]
        public void ThenOnlyLearnersWithCompletionPaymentsShouldHaveEventsGenerated()
        {
            ScenarioContext.Current.Pending();
        }


        private List<PaymentModel> CreatePayments(List<RecordedPayment> payments)
        {
            var returnPayments = new List<PaymentModel>();
            foreach (var recordedPayment in payments)
            {
                var payment = new PaymentModel()
                {
                   CollectionPeriod = recordedPayment.ParsedCollectionPeriod,
                   ContractType = recordedPayment.ContractType,
                   TransactionType = recordedPayment.Type,
                   Amount = recordedPayment.Amount,
                   DeliveryPeriod = 7,
                   Ukprn = 1000001,
                   JobId = 100000,
                   SfaContributionPercentage = 1m,
                   PriceEpisodeIdentifier = "pe-1",
                   FundingSource = FundingSourceType.CoInvestedEmployer,
                   LearningAimPathwayCode = 2,
                   LearnerReferenceNumber = "RefNo",
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