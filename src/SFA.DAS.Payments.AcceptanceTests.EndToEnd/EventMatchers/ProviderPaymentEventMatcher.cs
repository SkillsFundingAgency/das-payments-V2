using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class ProviderPaymentEventMatcher : BaseMatcher<ProviderPaymentEvent>
    {
        private readonly List<ProviderPayment> paymentSpec;
        private readonly TestSession testSession;
        private readonly CollectionPeriod collectionPeriod;


        public ProviderPaymentEventMatcher(CollectionPeriod collectionPeriod, TestSession testSession)
        {
            this.collectionPeriod = collectionPeriod;
            this.testSession = testSession;
        }

        public ProviderPaymentEventMatcher(CollectionPeriod collectionPeriod, TestSession testSession, List<ProviderPayment> paymentSpec)
            : this(collectionPeriod, testSession)
        {
            this.paymentSpec = paymentSpec;
        }

        protected override IList<ProviderPaymentEvent> GetActualEvents()
        {
            return ProviderPaymentEventHandler.ReceivedEvents
                .Where(ev =>
                    ev.Ukprn == testSession.Ukprn &&
                    ev.JobId == testSession.JobId &&
                    ev.CollectionPeriod.Name == collectionPeriod.Name)
                .ToList();

        }

        protected override IList<ProviderPaymentEvent> GetExpectedEvents()
        {
            var expectedPayments = new List<ProviderPaymentEvent>();

            foreach (var providerPayment in paymentSpec.Where(p => new CollectionPeriodBuilder().WithSpecDate(p.CollectionPeriod).Build().AcademicYear == collectionPeriod.Name))
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
                    var coFundedSfa = new SfaCoInvestedProviderPaymentEvent
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
                    var coFundedEmp = new EmployerCoInvestedProviderPaymentEvent
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
                    var fullyFundedSfa = new FullyFundedSfaProviderPaymentEvent
                    {
                        TransactionType = providerPayment.TransactionType,
                        AmountDue = providerPayment.SfaFullyFundedPayments,
                        CollectionPeriod = eventCollectionPeriod,
                        DeliveryPeriod = deliveryPeriod,
                        Learner = learner
                    };
                    expectedPayments.Add(fullyFundedSfa);
                }

            }

            return expectedPayments;
        }

        protected override bool Match(ProviderPaymentEvent expected, ProviderPaymentEvent actual)
        {
            return expected.GetType() == actual.GetType() &&
                   expected.TransactionType == actual.TransactionType &&
                   expected.AmountDue == actual.AmountDue &&
                   expected.CollectionPeriod.Name == actual.CollectionPeriod.Name &&
                   expected.DeliveryPeriod.Identifier == actual.DeliveryPeriod.Identifier &&
                   expected.Learner.ReferenceNumber == actual.Learner.ReferenceNumber &&
                   expected.Learner.Uln == actual.Learner.Uln;
        }
    }
}