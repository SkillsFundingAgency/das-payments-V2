using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data
{
    public class OnProgrammePaymentDue
    {
        public string PriceEpisodeIdentifier { get; set; }
        public  byte Delivery_Period { get; set; }
        public decimal Amount { get; set; }
        public OnProgrammeEarningType Type => TransactionType.Contains("Learning") ? 
            OnProgrammeEarningType.Learning :
            TransactionType.Contains("Completion") ? 
                OnProgrammeEarningType.Completion :
                OnProgrammeEarningType.Balancing;
        public string TransactionType { get; set; }
    }
}