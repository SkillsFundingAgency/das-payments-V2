using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Steps
{
    [Binding]
    public class ProviderResubmitsILRSteps : ProviderPaymentsStepsBase
    {
        protected long PreviousJobId { get => Get<long>("previous_job_id"); set => Set(value, "previous_job_id"); }

        public ProviderResubmitsILRSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"the provider has submitted an ILR file which has generated the following contract type (.*) payments:")]
        public async Task GivenTheProviderHasSubmittedAnILRFileWithJobIdWhichHasGeneratedTheFollowingPayments(ContractType contractType, Table table)
        {
            var submissionTime = DateTime.UtcNow.AddMinutes(-10);
            PreviousJobId = TestSession.GenerateId();
            ContractType = contractType;
            var previousPayments = table.CreateSet<FundingSourcePayment>().ToList();
            var payments = previousPayments.Select(p => CreatePayment(p, PreviousJobId, submissionTime)).ToList();
            var paymentDataContext = Container.Resolve<IPaymentsDataContext>();
            foreach (var payment in payments)
            {
                await paymentDataContext.Payment.AddAsync(payment);
            }
            try
            {
                await paymentDataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error storing the payments. Error: {ex.Message}.");
                Console.WriteLine(ex);
            }
            Console.WriteLine("Stored previous submission payments to the db.");
            var submissionJobSucceeded = new SubmissionJobSucceeded
            {
                Ukprn = TestSession.Ukprn,
                JobId = PreviousJobId,
                EventTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = submissionTime,
                CollectionPeriod = CollectionPeriod,
                AcademicYear = AcademicYear,
            };
            Console.WriteLine($"Sending the ilr submission event: {submissionJobSucceeded.ToJson()}");
            await MessageSession.Send(submissionJobSucceeded).ConfigureAwait(false);
        }

        private PaymentModel CreatePayment(FundingSourcePayment fundingSourcePayment, long jobId, DateTime? ilrSubmissionDate = null)
        {
            return new PaymentModel
            {
                FundingSource = fundingSourcePayment.FundingSourceType,
                IlrSubmissionDateTime = ilrSubmissionDate ?? DateTime.UtcNow,
                ContractType = ContractType,
                LearnerReferenceNumber = TestSession.Learner.LearnRefNumber,
                Ukprn = TestSession.Ukprn,
                TransactionType = fundingSourcePayment.Type,
                Amount = fundingSourcePayment.Amount,
                JobId = jobId,
                SfaContributionPercentage = SfaContributionPercentage,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(AcademicYear, CollectionPeriod),
                DeliveryPeriod = fundingSourcePayment.DeliveryPeriod,
                LearningAimPathwayCode = TestSession.Learner.Course.PathwayCode,
                LearningAimFrameworkCode = TestSession.Learner.Course.FrameworkCode,
                LearningAimFundingLineType = TestSession.Learner.Course.FundingLineType,
                LearningAimProgrammeType = TestSession.Learner.Course.ProgrammeType,
                LearningAimReference = TestSession.Learner.Course.LearnAimRef,
                LearningAimStandardCode = TestSession.Learner.Course.StandardCode,
                EventId = Guid.NewGuid(),
                LearnerUln = TestSession.Learner.Uln,
                PriceEpisodeIdentifier = "P1",

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
        }

        [When(@"the provider re-submits an ILR file which triggers the following contract type (.*) funding source payments:")]
        public async Task WhenTheProviderRe_SubmitsAnILRFileWhichTriggersTheFollowingContractTypeFundingSourcePayments(ContractType contractType, Table table)
        {
            var submissionTime = DateTime.UtcNow;
            var jobId = TestSession.JobId;
            var ilrSubmissionEvent = new SubmissionJobSucceeded
            {
                Ukprn = TestSession.Ukprn,
                JobId = jobId,
                EventTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = submissionTime,
                CollectionPeriod = CollectionPeriod,
                AcademicYear = AcademicYear,
            };
            Console.WriteLine($"Sending the ilr submission event: {ilrSubmissionEvent.ToJson()}");
            await MessageSession.Send(ilrSubmissionEvent).ConfigureAwait(false);

            ContractType = contractType;
            var fundingSourcePayments = table.CreateSet<FundingSourcePayment>().Select(p => CreateFundingSourcePaymentEvent(p, submissionTime)).ToList();
            foreach (var fundingSourcePaymentEvent in fundingSourcePayments)
            {
                Console.WriteLine($"Sending funding source event: {fundingSourcePaymentEvent.ToJson()}");
                await MessageSession.Send(fundingSourcePaymentEvent).ConfigureAwait(false);
            }
            Console.WriteLine("sent submission payments.");
        }

        [When(@"the provider re-submits an ILR file which triggers the following contract type (.*) funding source payments with the ILR Submission event sent after the payments:")]
        public async Task WhenTheProviderRe_SubmitsAnILRFileWhichTriggersTheFollowingContractTypeFundingSourcePaymentsWithTheILRSubmissionEventSentAfterThePayments(ContractType contractType, Table table)
        {
            var submissionTime = DateTime.UtcNow;
            var jobId = TestSession.JobId;

            ContractType = contractType;
            var fundingSourcePayments = table.CreateSet<FundingSourcePayment>().Select(p => CreateFundingSourcePaymentEvent(p, submissionTime)).ToList();
            foreach (var fundingSourcePaymentEvent in fundingSourcePayments)
            {
                Console.WriteLine($"Sending funding source event: {fundingSourcePaymentEvent.ToJson()}");
                await MessageSession.Send(fundingSourcePaymentEvent).ConfigureAwait(false);
            }
            Console.WriteLine("sent submission payments.");
            var ilrSubmissionEvent = new SubmissionJobSucceeded
            {
                Ukprn = TestSession.Ukprn,
                JobId = jobId,
                EventTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = submissionTime,
                CollectionPeriod = CollectionPeriod,
                AcademicYear = AcademicYear,
            };
            Console.WriteLine($"Sending the ilr submission event: {ilrSubmissionEvent.ToJson()}");
            await MessageSession.Send(ilrSubmissionEvent).ConfigureAwait(false);
        }

        [Then(@"the provider payments service should remove all payments for the previous Ilr submission")]
        public async Task ThenTheProviderPaymentsServiceShouldRemoveAllPaymentsForJobId()
        {
            var dataContext = Scope.Resolve<IPaymentsDataContext>();
            await WaitForIt(() => !dataContext.Payment.Any(p => p.JobId == PreviousJobId),
                $"The payments for the previous ILR submission were not removed.  Previous Job Id: {PreviousJobId}.");
        }
    }
}
