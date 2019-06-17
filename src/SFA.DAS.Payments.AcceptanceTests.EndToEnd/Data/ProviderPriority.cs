using TechTalk.SpecFlow.Assist.Attributes;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    class ProviderPriority
    {
        public long EmployerId { get; set; }
        public long Ukprn { get; set; }
        public int Priority { get; set; }
        public int AcademicYear { get; set; }
        public int CollectionPeriod { get; set; }
        [TableAliases("Collection_Period")]
        public string SpecCollectionPeriod { get; set; }
        [TableAliases("Provider")]
        public string ProviderIdentifier { get; set; }
    }
}
