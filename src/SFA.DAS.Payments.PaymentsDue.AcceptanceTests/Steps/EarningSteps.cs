using System;
using SFA.DAS.Payments.AcceptanceTests.Core;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class EarningSteps: StepsBase
    {
        public EarningSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"the SFA contribution percentage is (.*)%")]
        [When(@"the SFA contribution percentage changes to (.*)%")]
        public void SFAContributionPercentageIs(decimal sfaContributionPercentage)
        {
            Console.WriteLine($"Got sfa contribution percentage: {sfaContributionPercentage}");
            SfaContributionPercentage = sfaContributionPercentage / 100;
        }
    }
}
