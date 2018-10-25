using System;
using System.Collections.Generic;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class PaymentEventMatcher
    {
        public static Tuple<bool, string> MatchPayments(IList<Payment> expectedPayments, long ukprn)
        {
            throw new NotImplementedException();
        }
    }
}