using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Model.Core
{
    public class PriceEpisode
    {
        public string Identifier { get; set; }
        public decimal AgreedPrice { get; set; }
        public List<PriceEpisodePeriod> Periods { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}