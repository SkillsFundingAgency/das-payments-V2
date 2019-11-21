using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Entities
{
    public class DcValues
    {
        public DcValues()
        {
            DcContractTypeTotals = new List<DcContractTypeTotals>();
        }

        public List<DcContractTypeTotals> DcContractTypeTotals { get; set; }


        public decimal Total =>
            DcContractTypeTotals.Sum(t => t.TotalForContractType);
    }


    public class DcContractTypeTotals
    {
        public int ContractType { get; set; }
        public decimal TT1 { get; set; }
        public decimal TT2 { get; set; }
        public decimal TT3 { get; set; }
        public decimal TT4 { get; set; }
        public decimal TT5 { get; set; }
        public decimal TT6 { get; set; }
        public decimal TT7 { get; set; }
        public decimal TT8 { get; set; }
        public decimal TT9 { get; set; }
        public decimal TT10 { get; set; }
        public decimal TT11 { get; set; }
        public decimal TT12 { get; set; }
        public decimal TT13 { get; set; }
        public decimal TT14 { get; set; }
        public decimal TT15 { get; set; }
        public decimal TT16 { get; set; }


        public decimal TotalForContractType =>
            TT1 +
            TT2 +
            TT3 +
            TT4 +
            TT5 +
            TT6 +
            TT7 +
            TT8 +
            TT9 +
            TT10 +
            TT11 +
            TT12 +
            TT13 +
            TT14 +
            TT15 +
            TT16;
    }
}