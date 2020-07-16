using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model
{
    public class TransactionTypeAmountsByContractType
    {
        public ContractType ContractType { get; set; }
        public decimal TransactionType1 { get; set; }
        public decimal TransactionType2 { get; set; }
        public decimal TransactionType3 { get; set; }
        public decimal TransactionType4 { get; set; }
        public decimal TransactionType5 { get; set; }
        public decimal TransactionType6 { get; set; }
        public decimal TransactionType7 { get; set; }
        public decimal TransactionType8 { get; set; }
        public decimal TransactionType9 { get; set; }
        public decimal TransactionType10 { get; set; }
        public decimal TransactionType11 { get; set; }
        public decimal TransactionType12 { get; set; }
        public decimal TransactionType13 { get; set; }
        public decimal TransactionType14 { get; set; }
        public decimal TransactionType15 { get; set; }
        public decimal TransactionType16 { get; set; }
        public decimal Total => TransactionType1 +
                                TransactionType2 +
                                TransactionType3 +
                                TransactionType4 +
                                TransactionType5 +
                                TransactionType6 +
                                TransactionType7 +
                                TransactionType8 +
                                TransactionType9 +
                                TransactionType10 +
                                TransactionType11 +
                                TransactionType12 +
                                TransactionType13 +
                                TransactionType14 +
                                TransactionType15 +
                                TransactionType16;
    }
}