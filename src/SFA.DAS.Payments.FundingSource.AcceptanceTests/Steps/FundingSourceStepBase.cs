using System.Collections.Generic;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Data;
using TechTalk.SpecFlow;


namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    public class FundingSourceStepBase: StepsBase
    {

        public List<RequiredPayment> RequiredPayments { get => Get<List<RequiredPayment>>(); set => Set(value); }
        protected FundingSourceStepBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

    }
}
