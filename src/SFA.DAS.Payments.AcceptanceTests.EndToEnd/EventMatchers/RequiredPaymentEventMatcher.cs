using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class RequiredPaymentEventMatcher : BaseMatcher<RequiredPaymentEvent>
    {
        private readonly TestSession testSession;
        private readonly CalendarPeriod collectionPeriod;
        private readonly List<Payment> paymentSpec;

        public RequiredPaymentEventMatcher(TestSession testSession, CalendarPeriod collectionPeriod)
        {
            this.testSession = testSession;
            this.collectionPeriod = collectionPeriod;
        }

        public RequiredPaymentEventMatcher(TestSession testSession, CalendarPeriod collectionPeriod, List<Payment> paymentSpec) : this(testSession, collectionPeriod)
        {
            this.paymentSpec = paymentSpec;
        }

        protected override IList<RequiredPaymentEvent> GetActualEvents()
        {
            return RequiredPaymentEventHandler.ReceivedEvents
                .Where(e => e.Ukprn == testSession.Ukprn && e.CollectionPeriod == collectionPeriod && e.JobId == testSession.JobId).ToList();
        }

        protected override IList<RequiredPaymentEvent> GetExpectedEvents()
        {
            var expectedPayments = new List<RequiredPaymentEvent>();

            var paymentsToValidate =
                paymentSpec.Where(e => e.CollectionPeriod.ToCalendarPeriod().Name == collectionPeriod.Name);

            foreach (var payment in paymentsToValidate)
            {
                if (payment.OnProgramme != 0)
                {
                    var learningPayment = new ApprenticeshipContractType2RequiredPaymentEvent
                    {
                        AmountDue = payment.OnProgramme,
                        OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                        DeliveryPeriod = payment.DeliveryPeriod.ToCalendarPeriod()
                    };
                    expectedPayments.Add(learningPayment);
                }

                if (payment.Balancing != 0)
                {
                    var balancingPayment = new ApprenticeshipContractType2RequiredPaymentEvent
                    {
                        AmountDue = payment.Balancing,
                        OnProgrammeEarningType = OnProgrammeEarningType.Balancing,
                        DeliveryPeriod = payment.DeliveryPeriod.ToCalendarPeriod()
                    };

                    expectedPayments.Add(balancingPayment);
                }

                if (payment.Completion != 0)
                {
                    var completionPayment = new ApprenticeshipContractType2RequiredPaymentEvent
                    {
                        AmountDue = payment.Completion,
                        OnProgrammeEarningType = OnProgrammeEarningType.Completion,
                        DeliveryPeriod = payment.DeliveryPeriod.ToCalendarPeriod()
                    };
                    expectedPayments.Add(completionPayment);
                }

                foreach (var incentiveTypeKey in payment.IncentiveValues.Keys)
                {
                    var amount = payment.IncentiveValues[incentiveTypeKey];

                    if (amount != 0)
                        expectedPayments.Add(new IncentiveRequiredPaymentEvent
                        {
                            AmountDue = amount,
                            Type = incentiveTypeKey,
                            DeliveryPeriod = payment.DeliveryPeriod.ToCalendarPeriod()
                        });

                }    
            }

            return expectedPayments;
        }

        protected override bool Match(RequiredPaymentEvent expected, RequiredPaymentEvent actual)
        {
            if (expected.GetType() != actual.GetType())
                return false;

            return expected.DeliveryPeriod.Name == actual.DeliveryPeriod.Name &&
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