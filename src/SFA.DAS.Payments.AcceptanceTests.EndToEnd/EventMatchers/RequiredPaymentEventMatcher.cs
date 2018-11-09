using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public static class RequiredPaymentEventMatcher
    {
        public static Tuple<bool, string> MatchPayments(List<Payment> expectedPayments, long ukprn, CalendarPeriod collectionPeriod, long jobId)
        {
            var requiredPaymentEvents = ApprenticeshipContractType2RequiredPaymentEventHandler.ReceivedEvents
                .Where(e => e.Ukprn == ukprn && e.CollectionPeriod == collectionPeriod && e.JobId == jobId).ToList();

            expectedPayments = expectedPayments
                .Where(e => e.CollectionPeriod.ToDate().ToCalendarPeriod().Name == collectionPeriod.Name)
                .ToList();

            //TODO: refactor to reduce duplication
            if (!expectedPayments
                .Where(expected => expected.OnProgramme != 0)
                .All(expected => requiredPaymentEvents.Any(requiredPayment =>
                    requiredPayment.OnProgrammeEarningType == OnProgrammeEarningType.Learning &&
                    requiredPayment.AmountDue == expected.OnProgramme &&
                    requiredPayment.CollectionPeriod.Name == expected.CollectionPeriod.ToDate().ToCalendarPeriod().Name)))
            {
                return new Tuple<bool, string>(false, "Failed to find all on-programme (learning) payments.");
            }

            if (!expectedPayments
                .Where(expected => expected.Completion != 0)
                .All(expected => requiredPaymentEvents.Any(requiredPayment =>
                    requiredPayment.OnProgrammeEarningType == OnProgrammeEarningType.Completion &&
                    requiredPayment.AmountDue == expected.Completion &&
                    requiredPayment.CollectionPeriod.Name == expected.CollectionPeriod.ToDate().ToCalendarPeriod().Name)))
            {
                return new Tuple<bool, string>(false, "Failed to find all completion payments.");
            }

            if (!expectedPayments
                .Where(expected => expected.Balancing != 0)
                .All(expected => requiredPaymentEvents.Any(requiredPayment =>
                    requiredPayment.OnProgrammeEarningType == OnProgrammeEarningType.Balancing &&
                    requiredPayment.AmountDue == expected.Balancing &&
                    requiredPayment.CollectionPeriod.Name == expected.CollectionPeriod.ToDate().ToCalendarPeriod().Name)))
            {
                return new Tuple<bool, string>(false, "Failed to find all balancing payments.");
            }

            return new Tuple<bool, string>(true, string.Empty);


            //var receivedPayments = requiredPaymentEvents.Select(e =>
            //{
            //    var periodPayments = requiredPaymentEvents
            //        .Where(se => se.DeliveryPeriod == e.DeliveryPeriod)
            //        .ToDictionary(p => p.OnProgrammeEarningType, p => p.AmountDue);
            //    return new Payment
            //    {
            //        DeliveryPeriod = e.DeliveryPeriod.Name,
            //        Balancing = periodPayments[OnProgrammeEarningType.Balancing],
            //        Completion = periodPayments[OnProgrammeEarningType.Completion],
            //        OnProgramme = periodPayments[OnProgrammeEarningType.Learning]
            //    };
            //}).ToList();

            //var matchedPayments = receivedPayments
            //    .Where(receivedPayment => expectedPayments.Any(expectedEvent =>
            //        expectedEvent.DeliveryPeriod.ToCalendarPeriod().Name == receivedPayment.DeliveryPeriod
            //        && expectedEvent.OnProgramme == receivedPayment.OnProgramme
            //        && expectedEvent.Completion == receivedPayment.Completion
            //        && expectedEvent.Balancing == receivedPayment.Balancing
            //    )).ToList();

            //var allFound = matchedPayments.Count == expectedPayments.Count;
            //var nothingExtra = requiredPaymentEvents.Count == matchedPayments.Count;

            //var reason = new List<string>();
            //if (!allFound)
            //    reason.Add($"Did not find {expectedPayments.Count - matchedPayments.Count} out of {expectedPayments.Count} expected payments");
            //if (!nothingExtra)
            //    reason.Add($"Found {requiredPaymentEvents.Count - matchedPayments.Count} unexpected payments");

            //return new Tuple<bool, string>(allFound && nothingExtra, string.Join(" and ", reason));
        }
    }
}