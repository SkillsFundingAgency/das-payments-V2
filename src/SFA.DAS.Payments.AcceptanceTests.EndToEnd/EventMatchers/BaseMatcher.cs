using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public abstract class BaseMatcher<T>
    {
        protected abstract IList<T> GetActualEvents();

        protected abstract IList<T> GetExpectedEvents();

        protected abstract bool Match(T expected, T actual);

        public (bool pass, string reason, bool final) MatchPayments()
        {
            var actualPayments = GetActualEvents();

            var expectedPayments = GetExpectedEvents();

            // remove any FunctionalSkillEarningsEvent when we are not expecting any (only if they have all 0 values)
            if (expectedPayments.All(x => x.GetType() != typeof(FunctionalSkillEarningsEvent)))
            {
                actualPayments = RemoveEmptyFunctionSkillEarningEvent(actualPayments);
            }

            var matchedPayments = expectedPayments
                .Where(expected => actualPayments.Any(actual => Match(expected, actual)))
                .ToList();

            var errors = new List<string>();

            var final = false;

            if (matchedPayments.Count < expectedPayments.Count)
                errors.Add($"{expectedPayments.Count - matchedPayments.Count} out of {expectedPayments.Count} events were not found");

            if (actualPayments.Count > expectedPayments.Count)
            {
                errors.Add($"found {actualPayments.Count - expectedPayments.Count} unexpected events");
                final = true;
            }

            return (errors.Count == 0, string.Join(", ", errors), final);
        }

        public (bool pass, string reason) MatchNoPayments()
        {
            var payments = GetActualEvents();
            return !payments.Any()
                ? (true, string.Empty)
                : (false, $"Found Unexpected Payments: {payments.Aggregate(string.Empty, (currText, payment) => $"{currText}, {payment.ToJson()}")}");
        }

        private IList<T> RemoveEmptyFunctionSkillEarningEvent(IList<T> actualPayments)
        {
            return actualPayments.Except(actualPayments
                .Where(actualPayment => actualPayment.GetType() == typeof(FunctionalSkillEarningsEvent) && 
                                          (actualPayment as FunctionalSkillEarningsEvent).Earnings.All(e=>e.Periods.All(p=>p.Amount == 0)))).ToList();
        }
    }
}
