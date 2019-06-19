using TechTalk.SpecFlow.Assist.Attributes;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    class ProviderPriority
    {
        public int Priority { get; set; }
        [TableAliases("Collection_Period")]
        public string SpecCollectionPeriod { get; set; }
        [TableAliases("Provider")]
        public string ProviderIdentifier { get; set; }
    }
}
