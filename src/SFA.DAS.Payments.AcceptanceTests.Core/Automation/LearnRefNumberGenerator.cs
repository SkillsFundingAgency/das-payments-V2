using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    [Obsolete("Should use the auto generated learner reference on the TestLearner")]
    public class LearnRefNumberGenerator
    {
        private readonly int sessionIdHash;
        private readonly Dictionary<string, string> learnerLookup;

        public LearnRefNumberGenerator(string sessionId)
        {
            sessionIdHash = sessionId.GetHashCode();
            learnerLookup = new Dictionary<string, string>();
        }

        public string Generate(long ukprn, string learnRefNumber)
        {
            if (learnerLookup.ContainsKey(learnRefNumber))
            {
                return learnerLookup[learnRefNumber];
            }

            var generated = sessionIdHash
                            ^ ukprn.GetHashCode()
                            ^ learnRefNumber.GetHashCode();

            learnerLookup.Add(learnRefNumber, generated.ToString());

            return generated.ToString();
        }
    }
}