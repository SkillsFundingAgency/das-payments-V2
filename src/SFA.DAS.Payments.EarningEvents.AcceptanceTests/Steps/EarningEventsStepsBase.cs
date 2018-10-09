using System.Collections.Generic;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Model.Core;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    [Binding]
    public abstract class EarningEventsStepsBase: StepsBase
    {
        public List<PriceEpisode> PriceEpisodes { get => Get<List<PriceEpisode>>(); set => Set(value); }
        protected EarningEventsStepsBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }
    }
}