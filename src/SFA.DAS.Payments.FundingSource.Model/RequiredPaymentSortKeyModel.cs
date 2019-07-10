using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.FundingSource.Model
{
    public class RequiredPaymentSortKeyModel
    {
        public long Ukprn { get; set; }
        public DateTime AgreedOnDate { get; set; }
        public long Uln { get; set; }
        public string Id { get; set; }
    }
}
