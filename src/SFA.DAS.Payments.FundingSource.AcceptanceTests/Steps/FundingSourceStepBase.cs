using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class FundingSourceStepBase: StepsBase
    {

        public List<RequiredPayment> RequiredPayments { get ; set; }
        protected FundingSourceStepBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }


        [Given(@"the required payments component generates the following contract type (.*) payable earnings:")]
        public void GivenTheRequiredPaymentsComponentGeneratesTheFollowingContractTypePayableEarnings(byte contractType, Table payments)
        {
            ContractType = contractType;
            RequiredPayments = payments.CreateSet<RequiredPayment>().ToList();
        }

    }
}
