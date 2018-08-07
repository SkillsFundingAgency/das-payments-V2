using System.Linq;
using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class EarningSteps
    {
        private readonly ScenarioContext scenarioContext;

        public EarningSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given(@"the following earnings:")]
        public void GivenTheFollowingEarnings(Table table)
        {
        }

        [Given(@"the following contract type (.*) on programme earnings for periods (.*)-(.*):")]
        public void GivenTheFollowingContractTypeOnProgrammeEarningsForPeriods(short contractType, short fromPeriod, short toPeriod, Table table)
        {
            var rawEarnings = table.CreateSet<OnProgrammeEarning>();

            var contractTypeEarnings = new ContractTypeEarnings(contractType, fromPeriod, toPeriod, rawEarnings.ToList());

            scenarioContext[$"ContractType{contractType}OnProgrammeEarnings"] = contractTypeEarnings;
        }

        [Given(@"the following contract type (.*) incentive earnings for periods (.*)-(.*):")]
        public void GivenTheFollowingContractTypeIncentiveEarningsForPeriods(int contractType, int fromPeriod, int toPeriod, Table table)
        {
        }

        [Given(@"the following contract type (.*) incentive earnings for period (.*):")]
        public void GivenTheFollowingContractTypeIncentiveEarningsForPeriod(int contractType, int period, Table table)
        {
        }


        [Given(@"the following completion earnings:")]
        public void GivenTheFollowingCompletionEarnings(Table table)
        {
        }

        [Given(@"the following completion earnings for period (.*):")]
        public void GivenTheFollowingCompletionEarnings(int period, Table table)
        {
        }
    }
}
