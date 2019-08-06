using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.EarningEvents.Model.Entities
{
    public class SubmittedPriceEpisodeEntity
    {
        public long Id { get; set; }
        public long Ukprn { get; set; } // ilr, have
        public string LearnRefNumber { get; set; } // aim, have
        public string PriceEpisodeIdentifier { get; set; } // multiple per aim, have
        public IlrDetails IlrDetails { get; set; }
    }
}