using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    public class ExpectedEarning
    {
        public string PriceEpisodeIdentifier { get; set; }
        public int Period { get; set; }
        public OnProgrammeEarningType OnProgrammeEarningType { get; set; }
        public decimal Amount { get; set; }
    }
}