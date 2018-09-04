using SFA.DAS.Payments.FundingSource.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class RequiredPaymentsOutputEarnings
    {
        [Given(@"the payments due component generates the following contract type (.*) payable earnings:")]
        public void GivenThePaymentsDueComponentGeneratesTheFollowingContractTypeCompletionPayments(short contractType, Table table)
        {
            var completionEarnings = table.CreateSet<RequiredPayment>();

            ScenarioContext.Current[$"RequiredPaymentsContractType{contractType}CompletionPayments"] = completionEarnings;
        }

    }
}