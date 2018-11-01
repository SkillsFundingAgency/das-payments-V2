using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public static class PaymentEventMatcher
    {
        public static Tuple<bool, string> MatchPayments(IList<Payment> expectedPayments, long ukprn)
        {
            var sessionEvents = ApprenticeshipContractType2RequiredPaymentEventHandler.ReceivedEvents.Where(e => e.Ukprn == ukprn).ToArray();
            var receivedEvents = sessionEvents.Select(e =>
            {
                var periodPayments = sessionEvents.Where(se => se.DeliveryPeriod == e.DeliveryPeriod).ToDictionary(p => p.OnProgrammeEarningType, p => p.AmountDue);
                return new Payment
                {
                    DeliveryPeriod = e.DeliveryPeriod.Name,
                    Balancing = periodPayments[OnProgrammeEarningType.Balancing],
                    Completion = periodPayments[OnProgrammeEarningType.Completion],
                    OnProgramme = periodPayments[OnProgrammeEarningType.Learning]
                };
            }).ToArray();
            
            var matchedExpectations = receivedEvents
                .Where(receivedPayment => expectedPayments.Any(expectedEvent =>
                    expectedEvent.DeliveryPeriod == receivedPayment.DeliveryPeriod
                    && expectedEvent.OnProgramme == receivedPayment.OnProgramme
                    && expectedEvent.Completion == receivedPayment.Completion
                    && expectedEvent.Balancing == receivedPayment.Balancing
                )).ToList();

            var allFound = matchedExpectations.Count == expectedPayments.Count;
            var nothingExtra = sessionEvents.Length == matchedExpectations.Count;

            var reason = new List<string>();
            if (!allFound) 
                reason.Add($"Did not find {expectedPayments.Count - matchedExpectations.Count} out of {expectedPayments.Count} expected payments");
            if (!nothingExtra) 
                reason.Add($"Found {sessionEvents.Length - matchedExpectations.Count} unexpected payments");

            return new Tuple<bool, string>(allFound && nothingExtra, string.Join(" and ", reason));
        }
    }
}