using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-1826-DLock09-Two-Price-Episodes-Unhappy-Path")]
    // ReSharper disable once InconsistentNaming
    public class PV2_1826_Steps : FM36_ILR_Base_Steps
    {
        public PV2_1826_Steps(FeatureContext context) : base(context)
        {
        }

        [Given(@"there are 2 price episodes in the ILR submitted for (.*), PE-1 and PE-2")]
        public void GivenAreTwoPriceEpisodesInTheIlr(string collectionPeriodText)
        {
            GetFm36LearnerForCollectionPeriod(collectionPeriodText);
        }
    }
}