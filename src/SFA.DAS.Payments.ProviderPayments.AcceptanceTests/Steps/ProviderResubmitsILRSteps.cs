using NServiceBus;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
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

        [Given(@"the provider has submitted an ILR file which has generated the following contract type ""(.*)"" payments:")]
        public async Task GivenTheProviderHasSubmittedAnILRFileWithJobIdWhichHasGeneratedTheFollowingPayments(byte contractType, Table table)
        {
            var submissionTime = DateTime.UtcNow.AddMinutes(-10);
            PreviousJobId = TestSession.GenerateId("previous_job_id");
            ContractType = contractType;
            var previousPayments = table.CreateSet<FundingSourcePayment>().ToList();
            var payments = previousPayments.Select(p => CreatePayment(p, PreviousJobId, submissionTime)).ToList();
            foreach (var payment in payments)
            {
                DataContext.Payment.Add(payment);
            }

            DataContext.SaveChanges();
            Console.WriteLine("Stored previous submission payments to the db.");
            var ilrSubmissionEvent = new IlrSubmittedEvent
            {
                Ukprn = TestSession.Ukprn,
                JobId = PreviousJobId,
                EventTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = submissionTime,
                CollectionPeriod = new CalendarPeriod(GetYear(CollectionPeriod, CollectionYear).ToString(), CollectionPeriod)
            };
            Console.WriteLine($"Sending the ilr submission event: {ilrSubmissionEvent.ToJson()}");
            await MessageSession.Send(ilrSubmissionEvent).ConfigureAwait(false);
        }

        private PaymentModel CreatePayment(FundingSourcePayment fundingSourcePayment, long jobId, DateTime? ilrSubmissionDate = null)
        {
            return new PaymentModel
            {
                FundingSource = fundingSourcePayment.FundingSourceType,
                IlrSubmissionDateTime = ilrSubmissionDate ?? DateTime.UtcNow,
                ContractType = (ContractType)ContractType,
                LearnerReferenceNumber = TestSession.Learner.LearnRefNumber,
                Ukprn = TestSession.Ukprn,
                TransactionType = (TransactionType)fundingSourcePayment.Type,
                Amount = fundingSourcePayment.Amount,
                JobId = jobId,
                SfaContributionPercentage = SfaContributionPercentage,
                CollectionPeriod = new CalendarPeriod(GetYear(CollectionPeriod, CollectionYear).ToString(), CollectionPeriod),
                DeliveryPeriod = new CalendarPeriod(GetYear(fundingSourcePayment.DeliveryPeriod, CollectionYear).ToString(), fundingSourcePayment.DeliveryPeriod),
                LearningAimPathwayCode = TestSession.Learner.Course.PathwayCode,
                LearningAimFrameworkCode = TestSession.Learner.Course.FrameworkCode,
                LearningAimFundingLineType = TestSession.Learner.Course.FundingLineType,
                LearningAimProgrammeType = TestSession.Learner.Course.ProgrammeType,
                LearningAimReference = TestSession.Learner.Course.LearnAimRef,
                LearningAimStandardCode = TestSession.Learner.Course.StandardCode,
                ExternalId = Guid.NewGuid(),
                LearnerUln = TestSession.Learner.Uln,
                PriceEpisodeIdentifier = "P1"
            };

        }

        [When(@"the provider re-submits an ILR file which triggers the following contract type ""(.*)"" funding source payments:")]
        public async Task WhenTheProviderRe_SubmitsAnILRFileWhichTriggersTheFollowingContractTypeFundingSourcePayments(byte contractType, Table table)
        {
            var submissionTime = DateTime.UtcNow;
            var jobId = TestSession.JobId;
            var ilrSubmissionEvent = new IlrSubmittedEvent
            {
                Ukprn = TestSession.Ukprn,
                JobId = jobId,
                EventTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = submissionTime,
                CollectionPeriod = new CalendarPeriod(GetYear(CollectionPeriod, CollectionYear).ToString(), CollectionPeriod)
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

        [Then(@"the provider payments service should remove all payments for the previous Ilr submission")]
        public void ThenTheProviderPaymentsServiceShouldRemoveAllPaymentsForJobId()
        {
            WaitForIt(() => !DataContext.Payment.Any(p => p.JobId == PreviousJobId),
                $"The payments for the previous ILR submission were not removed.  Previous Job Id: {PreviousJobId}.");
        }
    }
}
