using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentTools.Model
{
    public class PriceEpisode
    {
        public PriceEpisode(string name, string act, decimal price, IEnumerable<Commitment> commitments, long ukprn,
            long earningLearnerUln, int? standardCode, int? frameworkCode, int? programmeType, int? pathwayCode, DateTimeOffset startDate)
        {
            EpisodeName = name;
            Act = act;
            Price = price;
            Commitments = commitments.ToList().AsReadOnly();
            Ukprn = ukprn;
            Uln = earningLearnerUln;
            StandardCode = standardCode;
            FrameworkCode = frameworkCode;
            ProgrammeType = programmeType;
            PathwayCode = pathwayCode;
            StartDate = startDate;
        }

        public string EpisodeName { get; }
        public string Act { get; }
        public decimal Price { get; }
        public IReadOnlyList<Commitment> Commitments { get; } = new List<Commitment>();
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public int? StandardCode { get; set; }
        public int? FrameworkCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? PathwayCode { get; set; }
        public DateTimeOffset StartDate { get; set; }
    }
}