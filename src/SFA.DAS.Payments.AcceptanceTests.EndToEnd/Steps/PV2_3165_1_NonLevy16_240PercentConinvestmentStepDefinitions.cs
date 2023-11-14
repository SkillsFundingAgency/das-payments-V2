using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.Model.Core;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class PV2_3165_1_NonLevy16_240PercentConinvestmentStepDefinitions: EndToEndStepsBase
    {
        public PV2_3165_1_NonLevy16_240PercentConinvestmentStepDefinitions(FeatureContext context) : base(context)
        {
        }

        [Given(@"the apprentice is employed by an On-Service employer who has never declared Levy")]
        public async Task GivenTheApprenticeIsEmployedByAnOn_ServiceEmployerWhoHasNeverDeclaredLevy()
        {
            var employer = TestSession.GetEmployer(TestSession.Employer.Identifier);
            employer.Balance = 0;
            employer.IsLevyPayer = true;
            await SaveLevyAccount(employer).ConfigureAwait(false);
        }

        [Given(@"the apprentice is doing an Apprenticeship which starts in January (.*)")]
        public void GivenTheApprenticeIsDoingAnApprenticeshipWhichStartsInJanuary(int p0)
        {
            throw new PendingStepException();
        }

        [Given(@"the apprentice is aged between (.*) and (.*) when the apprenticeship starts")]
        public void GivenTheApprenticeIsAgedBetweenAndWhenTheApprenticeshipStarts(int p0, int p1)
        {
            throw new PendingStepException();
        }

        [When(@"the provider submits the training details for the apprenticeship learning")]
        public void WhenTheProviderSubmitsTheTrainingDetailsForTheApprenticeshipLearning()
        {
            throw new PendingStepException();
        }

        [Then(@"the earnings generated by the earnings calc should have generated a default co-investment rate of (.*)%")]
        public void ThenTheEarningsGeneratedByTheEarningsCalcShouldHaveGeneratedADefaultCo_InvestmentRateOf(int p0)
        {
            throw new PendingStepException();
        }

    }
}