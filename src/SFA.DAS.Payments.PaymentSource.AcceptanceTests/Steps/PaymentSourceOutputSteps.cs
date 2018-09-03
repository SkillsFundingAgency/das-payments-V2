using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PaymentSource.AcceptanceTests.Steps
{
    [Binding]
    public class PaymentSourceOutputSteps
    {
        [Then(@"the payment source component will generate the following contract type (.*) coinvested payments:")]
        public void ThenThePaymentSourceComponentWillGenerateTheFollowingContractTypeCoinvestedPayments(short contractType, Table table)
        {
        }

    }
}