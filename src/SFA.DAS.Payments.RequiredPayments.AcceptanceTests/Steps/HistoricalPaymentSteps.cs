using System.Linq;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class HistoricalPaymentSteps
    {
        private readonly ScenarioContext context;

        public HistoricalPaymentSteps(ScenarioContext context)
        {
            this.context = context;
        }

        [Given(@"the following historical payments exist:")]
        public void GivenTheFollowingHistoricalPaymentsExist(Table table)
        {
        }

        [Given(@"the following historical contract type (.*) on programme payments exist:")]
        public void GivenTheFollowingHistoricalOnProgrammePaymentsExist(short contractType, Table table)
        {
            var payableEarnings = table.CreateSet<PayableEarning>().ToList();

            context.Set(payableEarnings);
        }

        [Given(@"the following historical contract type (.*) incentive payments exist:")]
        public void GivenTheFollowingHistoricalIncentivePaymentsExist(short contractType, Table table)
        {
        }

        [Given(@"the following historical contract type (.*) completion payment exist:")]
        public void GivenTheFollowingHistoricalContractTypeCompletionPaymentExist(short contractType, Table table)
        {
            
        }

        [Given(@"the following historical contract type (.*) On Programme Learning payments exist:")]
        public void GivenTheFollowingHistoricalContractTypeOnProgrammeLearningPaymentsExist(byte contractType, Table table)
        {
            
        }
    }
}