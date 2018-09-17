using SFA.DAS.Payments.FundingSource.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class RequiredPaymentsOutputEarnings
    {
        private readonly ScenarioContext context;

        public RequiredPaymentsOutputEarnings(ScenarioContext context)
        {
            this.context = context;
        }

        [Given(@"the payments due component generates the following contract type (.*) payable earnings:")]
        public void GivenThePaymentsDueComponentGeneratesTheFollowingContractTypePayableEarnings(short contractType, Table table)
        {
            var payableEarnings = table.CreateSet<RequiredPayment>();

            context.Set(payableEarnings);
        }

    }
}