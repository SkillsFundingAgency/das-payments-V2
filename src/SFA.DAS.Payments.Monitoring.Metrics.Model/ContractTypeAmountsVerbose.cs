namespace SFA.DAS.Payments.Monitoring.Metrics.Model
{
    public class ContractTypeAmountsVerbose: ContractTypeAmounts
    {
        public decimal DifferenceContractType1 { get; set; }
        public decimal DifferenceContractType2 { get; set; }
        public decimal DifferenceTotal => DifferenceContractType1 + DifferenceContractType2;
        public decimal PercentageContractType1 { get; set; }
        public decimal PercentageContractType2 { get; set; }
        public decimal Percentage => PercentageContractType1 + PercentageContractType2 != 0 ?
            (PercentageContractType1 + PercentageContractType2) / 2 : 0;
    }
}