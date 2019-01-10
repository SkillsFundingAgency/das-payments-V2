﻿using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
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

        [Given("\"(.*)\" previously submitted the following learner details")]
        public void GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetailsForProvider(
            string providerId,
            Table table)
        {
            GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetails(table);
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
            var earnings = CreateEarnings(table);

            PreviousEarnings = earnings;

            // for new style specs where no ILR specified
            if (PreviousIlr == null)
            {
                PreviousIlr = new List<Training>();
                foreach (var aim in TestSession.Learners.SelectMany(l => l.Aims).ToList())
                {
                    var firstPriceEpisode = aim.PriceEpisodes.First();

                    var training = new Training
                    {
                        AimReference = aim.AimReference,
                        AimSequenceNumber = aim.AimSequenceNumber,
                        ActualDuration = aim.ActualDuration,
                        //BalancingPayment = 
                        CompletionStatus = aim.CompletionStatus.ToString(),
                        ContractType = firstPriceEpisode.ContractType,
                        FrameworkCode = aim.FrameworkCode,
                        FundingLineType = aim.FundingLineType,
                        LearnerId = aim.LearnerId,
                        PathwayCode = aim.PathwayCode,
                        PlannedDuration = aim.PlannedDuration,
                        ProgrammeType = aim.ProgrammeType,
                        SfaContributionPercentage = firstPriceEpisode.SfaContributionPercentage,
                        StandardCode = aim.StandardCode,
                        StartDate = aim.StartDate,
                        TotalAssessmentPrice = firstPriceEpisode.TotalAssessmentPrice,
                        TotalTrainingPrice = firstPriceEpisode.TotalTrainingPrice,
                        Uln = TestSession.GetLearner(aim.LearnerId).Uln
                    };

                    PreviousIlr.Add(training);
                }
            }
        }

        [Given("the following payments had been generated for \"(.*)\"")]
        public async Task GivenTheFollowingProviderPaymentsHadBeenGenerated(string providerId, Table table)
        {
            await GivenTheFollowingProviderPaymentsHadBeenGenerated(table);
        }

        [Given(@"the following provider payments had been generated")]
        public async Task GivenTheFollowingProviderPaymentsHadBeenGenerated(Table table)
        {
            if (TestSession.AtLeastOneScenarioCompleted)
            {
                return;
            }

            var payments = table.CreateSet<ProviderPayment>().ToList();
            foreach (var payment in payments)
            {
                payment.Uln = TestSession.GetLearner(payment.LearnerId).Uln;
            }

            var previousJobId = TestSession.GenerateId();
            var previousSubmissionTime = DateTime.UtcNow.AddHours(-1);
            Console.WriteLine($"Previous job id: {previousJobId}");
            var previousPayments = payments.SelectMany(p =>
            {
                var learnerTraining = PreviousIlr;//.First(t => t.LearnerId == p.LearnerId);
                var learnerEarning =
                    PreviousEarnings.First(e => e.LearnerId == p.LearnerId && e.DeliveryPeriod == p.DeliveryPeriod);
                return CreatePayments(p, learnerTraining, previousJobId, previousSubmissionTime, learnerEarning);
            }).ToList();

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

        [Given("the Provider now changes the Learner's ULN to \"(.*)\"")]
        public void TheProviderChangesTheLearnersUln(long newUln)
        {
            TestSession.Learner.Uln = newUln;
            CurrentIlr = PreviousIlr;
        }

        [When(@"the amended ILR file is re-submitted for the learners in collection period (.*)")]
        [When(@"the ILR file is submitted for the learners for collection period (.*)")]
        public async Task WhenTheAmendedILRFileIsRe_SubmittedForTheLearnersInCollectionPeriodRCurrentAcademicYear(string collectionPeriod)
        {
            if (Context.ContainsKey("current_collection_period") && CurrentCollectionPeriod.Name != collectionPeriod.ToDate().ToCalendarPeriod().Name)
            {
                await RequiredPaymentsCacheCleaner.ClearCaches(TestSession);
                await Task.Delay(Config.TimeToPause);
            }

            SetCollectionPeriod(collectionPeriod);
        }

        [Then(@"only the following provider payments will be recorded")]
        public async Task ThenTheFollowingProviderPaymentsWillBeRecorded(Table table)
        {
            var expectedPayments = table.CreateSet<ProviderPayment>()
                .Where(p => p.CollectionPeriod.ToDate().ToCalendarPeriod().Name == CurrentCollectionPeriod.Name)
                .ToList();

            var dataContext = Container.Resolve<IPaymentsDataContext>();
            var contractType = CurrentIlr == null
                ? TestSession.Learners.First().Aims.First().PriceEpisodes.First().ContractType
                : CurrentIlr.First().ContractType;

            var matcher = new ProviderPaymentModelMatcher(dataContext, TestSession, CurrentCollectionPeriod.Name, expectedPayments, contractType);
            await WaitForIt(() => matcher.MatchPayments(), "Payment history check failure");
        }
    }
}