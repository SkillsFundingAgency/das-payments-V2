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
    public class FundingSourceStepBase: StepsBase
    {

        public List<RequiredPayment> RequiredPayments { get ; set; }
        protected FundingSourceStepBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }


       

    }
}
