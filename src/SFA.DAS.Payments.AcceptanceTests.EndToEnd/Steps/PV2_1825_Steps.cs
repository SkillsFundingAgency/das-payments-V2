using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-1825-DLock09-Two-Price-Episodes-Happy-Path")]
    // ReSharper disable once InconsistentNaming
    public class PV2_1825_Steps : FM36_ILR_Base_Steps
    {
        public PV2_1825_Steps(FeatureContext context) : base(context)
        {
        }

        [Given(@"there is an ILR for the collection period (.*) with 2 price episodes, the end date of one occurs in the same month as the start date of the other")]
        public void GivenThereIsAnIlrWith(string collectionPeriodText)
        {
            GetFm36LearnerForCollectionPeriod(collectionPeriodText);
        }

        [Given(@"the course in (.*) matches the course in Commitment (.*)")]
        public void GivenTheCourseInPeMatchesTheCourseInCommitment(string priceEpisodeIdentifier, string commitmentIdentifier)
        {
            //TODO: Copy course to apprenticeship 
        }

        [Then(@"the course in (.*) does not match the course for Commitment (.*)")]
        public void ThenTheCourseInPeDoesNotMatchTheCourseForCommitment(string priceEpisodeIdentifier, string commitmentIdentifier)
        {
            //TODO Assert Course do not match
        }
    }
}