using System.Collections.Generic;
using System.Linq;

namespace PaymentTools.Model
{
    public class PriceEpisode
    {
        public PriceEpisode(string name, string act, decimal price, IEnumerable<Commitment> commitments)
        {
            EpisodeName = name;
            Act = act;
            Price = price;
            Commitments = commitments.ToList().AsReadOnly();
        }

        public string EpisodeName { get; }
        public string Act { get; }
        public decimal Price { get; }
        public IReadOnlyList<Commitment> Commitments { get; } = new List<Commitment>();
    }
}