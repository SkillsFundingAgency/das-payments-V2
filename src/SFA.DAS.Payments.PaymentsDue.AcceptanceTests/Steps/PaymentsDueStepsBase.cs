using System.Collections.Generic;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public abstract class PaymentsDueStepsBase : StepsBase
    {
        public List<ApprenticeshipContractType2EarningEvent> Act2EarningEvents
        {
            get => Get<List<ApprenticeshipContractType2EarningEvent>>();
            set => Set(value);
        }

        protected PaymentsDueStepsBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }
    }
}