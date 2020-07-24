namespace SFA.DAS.Payments.Monitoring.Metrics.Model
{
    public class ContractTypeAmounts
    {
        public decimal ContractType1 { get; set; }
        public decimal ContractType2 { get; set; }
        public decimal Total => ContractType1 + ContractType2;

    }
}