using System.Linq;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class PaymentDueSteps : RequiredPaymentsStepsBase
    {
        public PaymentDueSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"the payments due component generates the following contract type (.*) payments due:")]
        public void GivenThePaymentsDueComponentGeneratesTheFollowingContractTypePaymentsDue(byte contractType, Table payments)
        {
            ContractType = contractType;
            PaymentsDue = payments.CreateSet<OnProgrammePaymentDue>()
                .ToList();
        }
    }
}