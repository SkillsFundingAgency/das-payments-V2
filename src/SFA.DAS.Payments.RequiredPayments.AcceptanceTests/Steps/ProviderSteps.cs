
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class ProviderSteps
    {
        [Given(@"an ILR for the provider (.*)")]
        public void GivenAnIlrForTheProvider(long ukprn)
        {
            ScenarioContext.Current["Ukprn"] = ukprn;
        }
    }
}