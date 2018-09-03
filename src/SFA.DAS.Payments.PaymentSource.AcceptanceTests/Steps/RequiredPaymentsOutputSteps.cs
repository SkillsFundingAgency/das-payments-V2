using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PaymentSource.AcceptanceTests.Steps
{
    [Binding]
    public class RequiredPaymentsOutputSteps
    {
        [Given(@"the payments due component generates the following contract type (.*) on programme earnings:")]
        public void GivenThePaymentsDueComponentGeneratesTheFollowingContractTypeOnProgrammeEarnings(short contractType, Table table)
        {
        }

        [Given(@"the payments due component generates the following contract type (.*) completion earnings:")]
        public void GivenThePaymentsDueComponentGeneratesTheFollowingContractTypeCompletionEarnings(short contractType, Table table)
        {
        }
    }
}