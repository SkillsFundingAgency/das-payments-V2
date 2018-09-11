using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class PaymentsDueOutputSteps
    {
        private readonly ScenarioContext context;

        public PaymentsDueOutputSteps(ScenarioContext context)
        {
            this.context = context;
        }
        [Then(@"the payments due component will generate the following payable earnings:")]
        public void ThenThePaymentsDueComponentWillGenerateTheFollowingPayableEarnings(Table table)
        {
        }

        [Then(@"the payments due component will generate the following contract type (.*) payable earnings:")]
        public void ThenThePaymentsDueComponentWillGenerateTheFollowingPayableEarnings(short contractType, Table table)
        {
            while (!context.ContainsKey("ContractType2PayableEarnings"))
            {

            }

            var payableEarnings = table.CreateSet<PayableEarning>().ToList();

            if (context["ContractType2PayableEarnings"] is List<PayableEarning> output && output.Any())
            {
                output.ForEach(o => payableEarnings.Contains(o));
            }
        }
    }
}