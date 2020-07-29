using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-2041-Fix-for-incorrect-net-negative-payments")]
    public class PV2_2041_steps : FM36_ILR_Base_Steps
    {
        public PV2_2041_steps(FeatureContext context) : base(context)
        {
        }

        [Given("the following payments exist")]
        public async Task TheFollowingPaymentExists(Table table)
        {
            var payments = table.CreateSet<ProviderPayment>().ToList();

            var learningDelivery = TestSession.FM36Global.Learners[0].LearningDeliveries
                .First(x => x.LearningDeliveryValues.LearnAimRef == "ZPROG001");
            var learner = TestSession.FM36Global.Learners[0];

            var providerPayments = payments.Select(x =>
            {
                var model = new PaymentModel
                {
                    EventId = Guid.NewGuid(),
                    JobId = TestSession.GenerateId(),
                    CollectionPeriod = x.ParsedCollectionPeriod,
                    ContractType = ContractType.Act2,
                    Ukprn = TestSession.FM36Global.UKPRN,
                    LearnerUln = learner.ULN,
                    PriceEpisodeIdentifier = TestSession.FM36Global.Learners[0].PriceEpisodes[1].PriceEpisodeIdentifier,
                    DeliveryPeriod = x.ParsedDeliveryPeriod.Period,
                    LearnerReferenceNumber = learner.LearnRefNumber,
                    LearningAimFrameworkCode = learningDelivery.LearningDeliveryValues.FworkCode ?? 0,
                    LearningAimFundingLineType = learningDelivery.LearningDeliveryValues.LearnDelInitialFundLineType,
                    LearningAimPathwayCode = learningDelivery.LearningDeliveryValues.PwayCode ?? 0,
                    LearningAimProgrammeType = learningDelivery.LearningDeliveryValues.ProgType ?? 0,
                    LearningAimReference = learningDelivery.LearningDeliveryValues.LearnAimRef,
                    LearningAimStandardCode = learningDelivery.LearningDeliveryValues.StdCode ?? 0,
                    SfaContributionPercentage = 0.9m,
                    TransactionType = x.TransactionType,
                    StartDate = new DateTime(2019, 1, 1),
                    IlrSubmissionDateTime = DateTime.Now,
                    EventTime = DateTimeOffset.Now,
                    CompletionStatus = 0,
                    AgreementId = string.Empty,
                    ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                    EarningEventId = Guid.Empty,
                    FundingSourceEventId = Guid.Empty,
                    ReportingAimFundingLineType = "16 - 18 Apprenticeship Non - Levy Contract(procured)"
                };
                if (x.SfaCoFundedPayments != 0)
                {
                    model.Amount = x.SfaCoFundedPayments;
                    model.FundingSource = FundingSourceType.CoInvestedSfa;
                }
                else if (x.SfaFullyFundedPayments != 0)
                {
                    model.Amount = x.SfaFullyFundedPayments;
                    model.FundingSource = FundingSourceType.FullyFundedSfa;
                    model.SfaContributionPercentage = 0;
                }
                else if (x.EmployerCoFundedPayments != 0)
                {
                    model.Amount = x.EmployerCoFundedPayments;
                    model.FundingSource = FundingSourceType.CoInvestedEmployer;
                }

                return model;
            }).ToList();

            await DataContext.Database.ExecuteSqlCommandAsync($"DELETE Payments2.Payment WHERE Ukprn = {TestSession.FM36Global.UKPRN} AND LearnerReferenceNumber = {learner.LearnRefNumber}");
            await DataContext.Payment.AddRangeAsync(providerPayments);
            await DataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        [Given("a learner has submitted an ilr")]
        public void ALearnerHasSubmittedAnIlr()
        {
            GetFm36LearnerForCollectionPeriod("R12/current academic year");
            TestSession.FM36Global.UKPRN = TestSession.Provider.Ukprn;
        }

        [When("the learner submits in R12")]
        public async Task TheLearnerSubmitsInR12()
        {
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.FM36Global.UKPRN,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await WaitForRequiredPayments(9);

            await EmployerMonthEndHelper.SendLevyMonthEndForEmployers(
                TestSession.GenerateId(),
                TestSession.Employers.Select(x => x.AccountId),
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                MessageSession);

            await WaitForPayments(11);
        }

        [Then(@"the resulting payments are not net negative")]
        public async Task ThenTheResultingPaymentsAreNotNetNegative()
        {
            await WaitForIt(() => HasCorrectlyFundedDeliveryPeriod3(), "Failed to find non net negative payments for delivery period 3");
        }

        private bool HasCorrectlyFundedDeliveryPeriod3()
        {
            var payments = Scope.Resolve<IPaymentsHelper>()
                .GetPayments(TestSession.Provider.Ukprn)
                .Where(x => x.DeliveryPeriod == 3 &&
                            x.Ukprn == TestSession.FM36Global.UKPRN &&
                            x.TransactionType == TransactionType.OnProgramme16To18FrameworkUplift);

            return payments.Sum(x => x.Amount) == 0m && payments.Any();
        }
    }
}