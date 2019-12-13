using System;

namespace EarningsComparer
{
    internal class CombinedRow
    {
        public CombinedRow(long ukprn, short apprenticeshipContractType)
        {
            Ukprn = ukprn;
            ApprenticeshipContractType = apprenticeshipContractType;
        }

        public long Ukprn { get; set; }
        public short ApprenticeshipContractType { get; set; }


        public EarningsRow DasRow { get; set; }
        public EarningsRow DcRow { get; set; }

        public bool TotalsEqual => Decimal.Round( DasRow?.AllTypes ??0m, 2) ==  Decimal.Round(DcRow?.AllTypes ??0m,2);

        public decimal TotalsDifference => (DcRow?.AllTypes ?? 0m) - (DasRow?.AllTypes ?? 0m);
    }


    internal class EarningsRow
    {
        public long Ukprn { get; set; }
        public short ApprenticeshipContractType { get; set; }
        public decimal AllTypes { get; set; }
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
        public decimal TT17 { get; set; }

    }
}