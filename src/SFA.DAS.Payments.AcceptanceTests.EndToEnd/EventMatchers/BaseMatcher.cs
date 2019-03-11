using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Core;

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
                ?  (true, string.Empty)
                :  (false, $"Found Unexpected Payments: {payments.Aggregate(string.Empty,(currText,payment)=> $"{currText}, {payment.ToJson()}")}");
        }
    }
}
