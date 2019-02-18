using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class FundingSourcePaymentEventMatcher : BaseMatcher<FundingSourcePaymentEvent>
    {
        private readonly List<ProviderPayment> paymentSpec;
        private readonly TestSession testSession;
        private readonly CollectionPeriod collectionPeriod;

        public FundingSourcePaymentEventMatcher(CollectionPeriod collectionPeriod, TestSession testSession)
        {
            this.collectionPeriod = collectionPeriod;
            this.testSession = testSession;
        }

        public FundingSourcePaymentEventMatcher(CollectionPeriod collectionPeriod, TestSession testSession, List<ProviderPayment> paymentSpec)
            : this(collectionPeriod, testSession)
        {
            this.paymentSpec = paymentSpec;
        }

        protected override IList<FundingSourcePaymentEvent> GetActualEvents()
        {
            return FundingSourcePaymentEventHandler.ReceivedEvents
                .Where(ev =>
                    ev.Ukprn == testSession.Ukprn &&
                    ev.JobId == testSession.JobId &&
                    ev.CollectionPeriod.Period == collectionPeriod.Period && 
                    ev.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear)
                .ToList();
        }

        protected override IList<FundingSourcePaymentEvent> GetExpectedEvents()
        {
            var expectedPayments = new List<FundingSourcePaymentEvent>();

            foreach (var providerPayment in paymentSpec.Where(p => p.ParsedCollectionPeriod.Period == collectionPeriod.Period 
                                                                   && p.ParsedCollectionPeriod.AcademicYear == collectionPeriod.AcademicYear))
            {
                var eventCollectionPeriod = new CollectionPeriodBuilder().WithSpecDate(providerPayment.CollectionPeriod).Build();
                var deliveryPeriod = new DeliveryPeriodBuilder().WithSpecDate(providerPayment.DeliveryPeriod).Build(); 
                var testLearner = testSession.GetLearner(providerPayment.LearnerId);
                var learner = new Learner
                {
                    ReferenceNumber = testLearner.LearnRefNumber,
                    Uln = testLearner.Uln,
                };

                if (providerPayment.SfaCoFundedPayments != 0)
                {
                    var coFundedSfa = new SfaCoInvestedFundingSourcePaymentEvent
                    {
                        TransactionType = providerPayment.TransactionType,
                        AmountDue = providerPayment.SfaCoFundedPayments,
                        CollectionPeriod = eventCollectionPeriod,
                        DeliveryPeriod = deliveryPeriod,
                        Learner = learner,
                        FundingSourceType = FundingSourceType.CoInvestedSfa
                    };
                    expectedPayments.Add(coFundedSfa);
                }

                if (providerPayment.EmployerCoFundedPayments != 0)
                {
                    var coFundedEmp = new EmployerCoInvestedFundingSourcePaymentEvent
                    {
                        TransactionType = providerPayment.TransactionType,
                        AmountDue = providerPayment.EmployerCoFundedPayments,
                        CollectionPeriod = eventCollectionPeriod,
                        DeliveryPeriod = deliveryPeriod,
                        Learner = learner,
                        FundingSourceType = FundingSourceType.CoInvestedEmployer
                    };
                    expectedPayments.Add(coFundedEmp);
                }

                if (providerPayment.SfaFullyFundedPayments != 0)
                {
                    var fullyFundedSfa = new SfaFullyFundedFundingSourcePaymentEvent
                    {
                        TransactionType = providerPayment.TransactionType,
                        AmountDue = providerPayment.SfaFullyFundedPayments,
                        CollectionPeriod = eventCollectionPeriod,
                        DeliveryPeriod = deliveryPeriod,
                        Learner = learner
                    };
                    expectedPayments.Add(fullyFundedSfa);
                }

                if (providerPayment.LevyPayments != 0)
                {
                    var levyFunded = new LevyFundingSourcePaymentEvent
                    {
                        TransactionType = providerPayment.TransactionType,
                        AmountDue = providerPayment.LevyPayments,
                        CollectionPeriod = eventCollectionPeriod,
                        DeliveryPeriod = deliveryPeriod,
                        Learner = learner
                    };
                    expectedPayments.Add(levyFunded);
                }
            }

            return expectedPayments;
        }

        protected override bool Match(FundingSourcePaymentEvent expected, FundingSourcePaymentEvent actual)
        {
            return expected.GetType() == actual.GetType() &&
                   expected.TransactionType == actual.TransactionType &&
                   expected.AmountDue == actual.AmountDue &&
                   expected.CollectionPeriod.Period == actual.CollectionPeriod.Period &&
                   expected.CollectionPeriod.AcademicYear == actual.CollectionPeriod.AcademicYear &&
                   expected.DeliveryPeriod == actual.DeliveryPeriod &&
                   expected.Learner.ReferenceNumber == actual.Learner.ReferenceNumber &&
                   expected.Learner.Uln == actual.Learner.Uln;
        }
    }
}