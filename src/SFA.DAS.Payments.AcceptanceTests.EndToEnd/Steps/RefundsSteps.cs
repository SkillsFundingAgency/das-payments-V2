using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Application.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
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
            AddTestLearners(PreviousIlr);
        }

        [Given(@"the following earnings had been generated for the learner")]
        public void GivenTheFollowingEarningsHadBeenGeneratedForTheLearner(Table table)
        {
            var earnings = table.CreateSet<OnProgrammeEarning>().ToList();
            PreviousEarnings = earnings;
        }

        [Given(@"the following provider payments had been generated")]
        public async Task GivenTheFollowingProviderPaymentsHadBeenGenerated(Table table)
        {
            var payments = table.CreateSet<ProviderPayment>().ToList();

            var previousJobId = TestSession.GenerateId();
            var previousSubmissionTime = DateTime.UtcNow.AddHours(-1);
            Console.WriteLine($"Previous job id: {previousJobId}");
            var previousPayments = payments.SelectMany(p =>
            {
                var learnerTraining = PreviousIlr.First(t => t.LearnerId == p.LearnerId);
                var learnerEarning =
                    PreviousEarnings.First(e => e.LearnerId == p.LearnerId && e.DeliveryPeriod == p.DeliveryPeriod);
                return CreatePayments(p, learnerTraining, previousJobId, previousSubmissionTime, learnerEarning);
            });

            var dataContext = Container.Resolve<IPaymentsDataContext>();
            var currentHistory = await dataContext.Payment.Where(p => p.Ukprn == TestSession.Ukprn).ToListAsync();

            previousPayments = previousPayments
                .Where(p => !currentHistory.Any(historicPayment =>
                    historicPayment.LearnerReferenceNumber == p.LearnerReferenceNumber &&
                    historicPayment.TransactionType == p.TransactionType &&
                    historicPayment.DeliveryPeriod.Name == p.DeliveryPeriod.Name))
                .ToList();

            dataContext.Payment.AddRange(previousPayments);
            await dataContext.SaveChangesAsync();
        }

        [Given(@"the Provider now changes the Learner details as follows")]
        public void GivenTheProviderNowChangesTheLearnerDetailsAsFollows(Table table)
        {
            var ilr = table.CreateSet<Training>().ToList();
            CurrentIlr = ilr;
            AddTestLearners(CurrentIlr);
        }

        [Given(@"price details as follows")]
        public void GivenPriceDetailsAsFollows(Table table)
        {
            CurrentPriceEpisodes = table.CreateSet<Price>().ToList();
        }

        [When(@"the amended ILR file is re-submitted for the learners in collection period (.*)")]
        [When(@"the ILR file is submitted for the learners for collection period (.*)")]
        public async Task WhenTheAmendedILRFileIsRe_SubmittedForTheLearnersInCollectionPeriodRCurrentAcademicYear(string collectionPeriod)
        {
            if (Context.ContainsKey("current_collection_period") && CurrentCollectionPeriod.Name != collectionPeriod.ToDate().ToCalendarPeriod().Name)
                await RequiredPaymentsCacheCleaner.ClearCaches(TestSession);

            SetCollectionPeriod(collectionPeriod);
        }

        [Then(@"only the following provider payments will be recorded")]
        public async Task ThenTheFollowingProviderPaymentsWillBeRecorded(Table table)
        {
            var expectedPayments = table.CreateSet<ProviderPayment>()
                .Where(p => p.CollectionPeriod.ToDate().ToCalendarPeriod().Name == CurrentCollectionPeriod.Name)
                .ToList();

            var dataContext = Container.Resolve<IPaymentsDataContext>();
            var matcher = new ProviderPaymentModelMatcher(dataContext, TestSession, CurrentCollectionPeriod.Name, expectedPayments, CurrentIlr.First().ContractType);
            await WaitForIt(() => matcher.MatchPayments(), "Payment history check failure");
        }
    }
}