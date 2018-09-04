using SFA.DAS.Payments.FundingSource.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class FundingSourceOutputSteps
    {
        [Then(@"the payment source component will generate the following contract type (.*) coinvested payments:")]
        public void ThenThePaymentSourceComponentWillGenerateTheFollowingContractTypeCoinvestedPayments(short contractType, Table table)
        {
            var payments = table.CreateSet<FundingSourcePayment>();
        }

    }
}