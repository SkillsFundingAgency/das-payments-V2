using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Steps
{
    [Binding]
    public class MonthEndCompletionPaymentSteps :ProviderPaymentsStepsBase
    {
        public MonthEndCompletionPaymentSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
            
        }


        
        [Then(@"DAS approvals service should be notified of payments for learners with completion payments")]
        public void ThenDASApprovalsServiceShouldBeNotifiedOfPaymentsForLearnersWithCompletionPayments()
        {
            ScenarioContext.Current.Pending();
        }

    }
}