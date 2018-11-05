using System;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class RefundsSteps
    {
        [Given(@"the provider perviously submitted the following learner details")]
        public void GivenTheProviderPerviouslySubmittedTheFollowingLearnerDetails(Table table)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
