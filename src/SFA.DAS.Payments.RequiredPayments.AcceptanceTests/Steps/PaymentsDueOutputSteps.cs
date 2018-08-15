using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class PaymentsDueOutputSteps
    {
        private readonly ScenarioContext scenarioContext;

        public PaymentsDueOutputSteps(ScenarioContext context)
        {
            scenarioContext = context;
        }

        [Then(@"the payments due component will generate the following payable earnings:")]
        public void ThenThePaymentsDueComponentWillGenerateTheFollowingPayableEarnings(Table table)
        {
        }

        [Then(@"the payments due component will generate the following contract type (.*) payable earnings:")]
        public void ThenThePaymentsDueComponentWillGenerateTheFollowingPayableEarnings(short contractType, Table table)
        {
        }
    }
}