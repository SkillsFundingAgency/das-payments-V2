using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class RequiredPaymentEventMatcher : BaseMatcher<ApprenticeshipContractType2RequiredPaymentEvent>
    {        
        private readonly TestSession testSession;
        private readonly CalendarPeriod collectionPeriod;
        private readonly List<Payment> paymentSpec;

        public RequiredPaymentEventMatcher(TestSession testSession, CalendarPeriod collectionPeriod, List<Payment> paymentSpec)
        {
            this.testSession = testSession;
            this.collectionPeriod = collectionPeriod;
            this.paymentSpec = paymentSpec;
        }

        protected override IList<ApprenticeshipContractType2RequiredPaymentEvent> GetActualEvents()
        {
            return ApprenticeshipContractType2RequiredPaymentEventHandler.ReceivedEvents
                .Where(e => e.Ukprn == testSession.Ukprn && e.CollectionPeriod == collectionPeriod && e.JobId == testSession.JobId).ToList();
        }

        protected override IList<ApprenticeshipContractType2RequiredPaymentEvent> GetExpectedEvents()
        {
            var expectedPayments = new List<ApprenticeshipContractType2RequiredPaymentEvent>();

            foreach (var payment in paymentSpec.Where(e => e.CollectionPeriod.ToDate().ToCalendarPeriod().Name == collectionPeriod.Name))
            {
                var learningPayment = new ApprenticeshipContractType2RequiredPaymentEvent
                {
                    AmountDue = payment.OnProgramme,
                    OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                    DeliveryPeriod = payment.DeliveryPeriod.ToCalendarPeriod()
                };
                var balancingPayment = new ApprenticeshipContractType2RequiredPaymentEvent
                {
                    AmountDue = payment.Balancing,
                    OnProgrammeEarningType = OnProgrammeEarningType.Balancing,
                    DeliveryPeriod = payment.DeliveryPeriod.ToCalendarPeriod()
                };
                var completionPayment = new ApprenticeshipContractType2RequiredPaymentEvent
                {
                    AmountDue = payment.Completion,
                    OnProgrammeEarningType = OnProgrammeEarningType.Completion,
                    DeliveryPeriod = payment.DeliveryPeriod.ToCalendarPeriod()
                };

                if (learningPayment.AmountDue > 0) 
                    expectedPayments.Add(learningPayment);

                if (balancingPayment.AmountDue > 0) 
                    expectedPayments.Add(balancingPayment);

                if (completionPayment.AmountDue > 0) 
                    expectedPayments.Add(completionPayment);
            }

            return expectedPayments;
        }

        protected override bool Match(ApprenticeshipContractType2RequiredPaymentEvent expected, ApprenticeshipContractType2RequiredPaymentEvent actual)
        {
            return expected.DeliveryPeriod.Name == actual.DeliveryPeriod.Name &&
                   expected.OnProgrammeEarningType == actual.OnProgrammeEarningType &&
                   expected.AmountDue == actual.AmountDue;
        }
    }
}