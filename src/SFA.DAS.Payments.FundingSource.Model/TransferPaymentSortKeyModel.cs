using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.FundingSource.Model
{
    public class TransferPaymentSortKeyModel
    {
        public string Id { get; set; }
        public DateTime AgreedOnDate { get; set; }
        public long Uln { get; set; }
    }
}
