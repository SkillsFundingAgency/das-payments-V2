using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class RequiredPaymentEventMatcher : BaseMatcher<RequiredPaymentEvent>
    {        
        private readonly TestSession testSession;
        private readonly CollectionPeriod collectionPeriod;
        private readonly List<Payment> paymentSpec;

        public RequiredPaymentEventMatcher(TestSession testSession, CollectionPeriod collectionPeriod)
        {
            this.testSession = testSession;
            this.collectionPeriod = collectionPeriod;
        }

        public RequiredPaymentEventMatcher(TestSession testSession, CollectionPeriod collectionPeriod, List<Payment> paymentSpec) : this(testSession, collectionPeriod)
        {
            this.paymentSpec = paymentSpec;
        }

        protected override IList<RequiredPaymentEvent> GetActualEvents()
        {
            return RequiredPaymentEventHandler.ReceivedEvents
                .Where(e => e.Ukprn == testSession.Ukprn && 
                            e.CollectionPeriod.Period== collectionPeriod.Period &&
                            e.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear &&
                            e.JobId == testSession.JobId).ToList();
        }

        protected override IList<RequiredPaymentEvent> GetExpectedEvents()
        {
            var expectedPayments = new List<RequiredPaymentEvent>();

            var paymentsToValidate =
                paymentSpec.Where(e => e.ParsedCollectionPeriod.AcademicYear == collectionPeriod.AcademicYear && e.ParsedCollectionPeriod.Period == collectionPeriod.Period)
                    .ToList();

            foreach (var payment in paymentsToValidate)
            {
                AddOnProgPayment(payment, expectedPayments, payment.OnProgramme, OnProgrammeEarningType.Learning);
                AddOnProgPayment(payment, expectedPayments, payment.Balancing, OnProgrammeEarningType.Balancing);
                AddOnProgPayment(payment, expectedPayments, payment.Completion, OnProgrammeEarningType.Completion);

                foreach (var incentiveTypeKey in payment.IncentiveValues.Keys)
                {
                    var amount = payment.IncentiveValues[incentiveTypeKey];

                    if (amount != 0)
                        expectedPayments.Add(new IncentiveRequiredPaymentEvent
                        {
                            AmountDue = amount,
                            Type = incentiveTypeKey,
                            DeliveryPeriod = new DeliveryPeriodBuilder().WithSpecDate(payment.DeliveryPeriod).Build(),
                        });

                }    
            }

            return expectedPayments;
        }

        private static void AddOnProgPayment(Payment paymentSpec, List<RequiredPaymentEvent> expectedPayments, decimal amountDue, OnProgrammeEarningType type)
        {
            var payment = new ApprenticeshipContractType2RequiredPaymentEvent
            {
                AmountDue = amountDue,
                OnProgrammeEarningType = type,
                DeliveryPeriod = new DeliveryPeriodBuilder().WithSpecDate(paymentSpec.DeliveryPeriod).Build(),
            };

            if (payment.AmountDue != 0)
                expectedPayments.Add(payment);
        }

        protected override bool Match(RequiredPaymentEvent expected, RequiredPaymentEvent actual)
        {
            if (expected.GetType() != actual.GetType())
                return false;

            return expected.DeliveryPeriod == actual.DeliveryPeriod &&
                   expected.AmountDue == actual.AmountDue &&
                   MatchAct(expected as ApprenticeshipContractTypeRequiredPaymentEvent, actual as ApprenticeshipContractTypeRequiredPaymentEvent) &&
                   MatchIncentive(expected as IncentiveRequiredPaymentEvent, actual as IncentiveRequiredPaymentEvent);
        }

        private bool MatchAct(ApprenticeshipContractTypeRequiredPaymentEvent expected, ApprenticeshipContractTypeRequiredPaymentEvent actual)
        {
            if (expected == null)
                return true;

            return expected.OnProgrammeEarningType == actual.OnProgrammeEarningType;
        }

        private bool MatchIncentive(IncentiveRequiredPaymentEvent expected, IncentiveRequiredPaymentEvent actual)
        {
            if (expected == null)
                return true;

            return expected.Type == actual.Type;
        }
    }
}