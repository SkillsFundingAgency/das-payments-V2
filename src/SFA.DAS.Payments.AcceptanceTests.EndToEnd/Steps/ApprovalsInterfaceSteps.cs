using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class ApprovalsInterfaceSteps : EndToEndStepsBase
    {
        public Employer 

        public ApprovalsInterfaceSteps(FeatureContext context) : base(context)
        {
        }

        [Given(@"the following employer")]
        public void GivenTheFollowingEmployer(Table employers)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"the following apprenticehsips have been approved")]
        public void GivenTheFollowingApprenticehsipsHaveBeenApproved(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"the apprenticeships have the following price episodes")]
        public void GivenTheApprenticeshipsHaveTheFollowingPriceEpisodes(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the Approvals service notifies the Payments service of the apprenticeships")]   
        public void WhenTheApprovalsServiceNotifiesPaymentsVOfTheApprenticeships(int p0)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the Payments service should record the apprenticeships")]
        public void ThenPaymentsVShouldRecordTheApprenticeships(int p0)
        {
            ScenarioContext.Current.Pending();
        }

    }
}