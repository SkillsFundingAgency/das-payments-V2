using System.Collections.Generic;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public abstract class RequiredPaymentsStepsBase: StepsBase
    {
        public List<OnProgrammeEarning> Earnings { get => Get<List<OnProgrammeEarning>>(); set => Set(value); }
        protected RequiredPaymentsStepsBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }
    }
}