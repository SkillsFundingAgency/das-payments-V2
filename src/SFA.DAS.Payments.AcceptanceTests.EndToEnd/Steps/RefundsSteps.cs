using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Application.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class RefundsSteps : EndToEndStepsBase
    {
        public RefundsSteps(FeatureContext context) : base(context)
        {
        }

        [Given(@"the provider previously submitted the following learner details")]
        public void GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetails(Table table)
        {
            var ilr = table.CreateSet<Training>().ToList();
            PreviousIlr = ilr;
        }

        [Given(@"the following earnings had been generated for the learner")]
        public void GivenTheFollowingEarningsHadBeenGeneratedForTheLearner(Table table)
        {
            var earnings = table.CreateSet<OnProgrammeEarning>().ToList();
            PreviousEarnings = earnings;
        }

        [Given(@"the following provider payments had been generated")]
        public void GivenTheFollowingProviderPaymentsHadBeenGenerated(Table table)
        {
            var previousJobId = TestSession.GenerateId();
            var previousSubmissionTime = DateTime.UtcNow.AddHours(-1);
            Console.WriteLine($"Previous job id: {previousJobId}");
            var payments = table.CreateSet<ProviderPayment>().ToList();
            var previousPayments = payments.SelectMany(p =>
            {
                var learnerTraining = PreviousIlr.First(t => t.LearnerId == p.LearnerId);
                var learnerEarning = PreviousEarnings.First(e => e.LearnerId == p.LearnerId && e.DeliveryPeriod == p.DeliveryPeriod);
                return CreatePayments(p, learnerTraining, previousJobId, previousSubmissionTime, learnerEarning);
            });

            var dataContext = Container.Resolve<IPaymentsDataContext>();
            dataContext.Payment.AddRange(previousPayments);
            dataContext.SaveChanges();
        }

        [Given(@"the Provider now changes the Learner details as follows")]
        public void GivenTheProviderNowChangesTheLearnerDetailsAsFollows(Table table)
        {
            var ilr = table.CreateSet<Training>().ToList();
            CurrentIlr = ilr;
        }

        [When(@"the amended ILR file is re-submitted for the learners in collection period (.*)")]
        [When(@"the ILR file is submitted for the learners for collection period (.*)")]
        public async Task WhenTheAmendedILRFileIsRe_SubmittedForTheLearnersInCollectionPeriodRCurrentAcademicYear(string collectionPeriod)
        {
            SetCollectionPeriod(collectionPeriod);
        }

        [Then(@"the following provider payments will be recorded")]
        public async Task ThenTheFollowingProviderPaymentsWillBeRecorded(Table table)
        {
            var expectedPayments = table.CreateSet<ProviderPayment>()
                .Where(p => p.CollectionPeriod.ToDate().ToCalendarPeriod().Name == CurrentCollectionPeriod.Name)
                .ToList();

            var dataContext = Container.Resolve<IPaymentsDataContext>();
            await WaitForIt(() =>
            {
                var payments = dataContext.Payment.Where(p => p.JobId == TestSession.JobId &&
                                                              p.CollectionPeriod.Name == CurrentCollectionPeriod.Name).ToList();

                Console.WriteLine($"Found {payments.Count} recorded payments for job id: {TestSession.JobId}, learner ref: {TestSession.Learner.LearnRefNumber}");

                return expectedPayments.All(expected => payments.Any(p => expected.CollectionPeriod.ToDate().ToCalendarPeriod().Name == p.CollectionPeriod.Name &&
                                                                          TestSession.GenerateLearnerReference(expected.LearnerId) == p.LearnerReferenceNumber &&
                                                                          expected.TransactionType == p.TransactionType &&
                                                                          CurrentIlr.First().ContractType == p.ContractType &&
                                                                          (p.FundingSource == FundingSourceType.CoInvestedSfa && expected.SfaCoFundedPayments == p.Amount ||
                                                                           p.FundingSource == FundingSourceType.CoInvestedEmployer && expected.EmployerCoFundedPayments == p.Amount)));

            }, "Failed to find all the expected stored provider payments.");
        }
    }
}
