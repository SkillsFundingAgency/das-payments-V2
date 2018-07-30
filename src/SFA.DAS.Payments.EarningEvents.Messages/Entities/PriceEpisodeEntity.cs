using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.EarningEvents.Messages.Entities
{
    public class PriceEpisodeEntity
    {
        public decimal Price { get; set; }

        public IEnumerable<byte> Periods { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
