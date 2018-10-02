using SFA.DAS.Payments.AcceptanceTests.Core;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class SmallEmployerPaymentsSteps : StepsBase
    {
        public SmallEmployerPaymentsSteps(ScenarioContext context) : base(context)
        {
            ContractType = 2;
        }
        
        [Given(@"the required payments component generates the following contract type (.*) payable earnings:")]
        public void GivenTheRequiredPaymentsComponentGeneratesTheFollowingContractTypePayableEarnings(int p0, Table table)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"required payments event is received")]
        public void WhenRequiredPaymentsEventIsReceived()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the payment source component will generate the following contract type (.*) coinvested payments:")]
        public void ThenThePaymentSourceComponentWillGenerateTheFollowingContractTypeCoinvestedPayments(int p0, Table table)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
