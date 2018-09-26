using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class LearnRefNumberGenerator
    {
        private readonly ScenarioContext context;
        private readonly Dictionary<string, string> learnerLookup;

        public LearnRefNumberGenerator(ScenarioContext context)
        {
            this.context = context;
            learnerLookup = new Dictionary<string, string>();
        }

        public string Generate(long ukprn, string learnRefNumber)
        {
            if (learnerLookup.ContainsKey(learnRefNumber))
            {
                return learnerLookup[learnRefNumber];
            }

            var sessionId = (Guid)context["SessionId"];

            var generated = sessionId.GetHashCode()
                            ^ ukprn.GetHashCode()
                            ^ learnRefNumber.GetHashCode();

            learnerLookup.Add(learnRefNumber, generated.ToString());

            return generated.ToString();
        }
    }
}
