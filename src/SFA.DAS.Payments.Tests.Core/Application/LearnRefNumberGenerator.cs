using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.Tests.Core.Application
{
    public class LearnRefNumberGenerator
    {
        private readonly ScenarioContext scenarioContext;

        private readonly Dictionary<string, int> learnerLookup;

        public LearnRefNumberGenerator(ScenarioContext context)
        {
            scenarioContext = context;

            learnerLookup = new Dictionary<string, int>();
        }

        public long Generate(long ukprn, string learnRefNumber)
        {
            if (learnerLookup.ContainsKey(learnRefNumber))
            {
                return learnerLookup[learnRefNumber];
            }

            var sessionId = (Guid)scenarioContext["SessionId"];

            var generated = sessionId.GetHashCode()
                            ^ ukprn.GetHashCode()
                            ^ learnRefNumber.GetHashCode();

            learnerLookup.Add(learnRefNumber, generated);

            return generated;
        }
    }
}
