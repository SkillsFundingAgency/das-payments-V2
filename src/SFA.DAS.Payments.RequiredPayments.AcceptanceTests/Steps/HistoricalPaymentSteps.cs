using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class HistoricalPaymentSteps
    {
        [Given(@"the following historical payments exist:")]
        public void GivenTheFollowingHistoricalPaymentsExist(Table table)
        {
        }

        [Given(@"the following historical contract type (.*) on programme payments exist:")]
        public void GivenTheFollowingHistoricalOnProgrammePaymentsExist(short contractType, Table table)
        {
        }

        [Given(@"the following historical contract type (.*) incentive payments exist:")]
        public void GivenTheFollowingHistoricalIncentivePaymentsExist(short contractType, Table table)
        {
        }

        [Given(@"the following historical contract type (.*) completion payment exist:")]
        public void GivenTheFollowingHistoricalContractTypeCompletionPaymentExist(short contractType, Table table)
        {
            
        }


    }
}