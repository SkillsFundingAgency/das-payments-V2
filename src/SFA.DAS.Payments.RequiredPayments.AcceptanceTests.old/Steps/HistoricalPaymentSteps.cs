using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class HistoricalPaymentSteps
    {
        [Given(@"the following historical payments exist:")]
        public void GivenTheFollowingHistoricalPaymentsExist(Table table)
        {
        }

        [Given(@"the following historical contract type (.*) payments exist:")]
        public void GivenTheFollowingHistoricalPaymentsExist(short contractType, Table table)
        {
        }

    }
}