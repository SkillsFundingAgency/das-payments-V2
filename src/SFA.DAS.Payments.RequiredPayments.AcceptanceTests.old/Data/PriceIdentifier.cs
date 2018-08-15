using System;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data
{
    public abstract class PriceIdentifier : LearnerIdentifier
    {
        public string PriceEpisodeIdentifier { get; set; }

        public DateTime EpisodeStartDate { get; set; }

        public DateTime EpisodeEffectiveTNPStartDate { get; set; }

        public decimal TotalNegotiatedPrice { get; set; }
    }
}