using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-1797-DLock09-One-Price-Episode-Happy-Path")]
    // ReSharper disable once InconsistentNaming
    public class PV2_1797_Steps : FM36_ILR_Base_Steps
    {
        public PV2_1797_Steps(FeatureContext context) : base(context)
        {
        }
    }
}