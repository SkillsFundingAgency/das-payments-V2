using System.Collections.Generic;

namespace SFA.DAS.Payments.Model.Core
{
    public class PriceEpisodePeriod
    {
        public List<byte> Periods { get; set; }
        public decimal Amount { get; set; }
    }
}