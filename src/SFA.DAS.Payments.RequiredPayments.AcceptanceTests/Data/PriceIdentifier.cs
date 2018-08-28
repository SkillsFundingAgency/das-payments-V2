using System;
using SFA.DAS.Payments.Tests.Core.Data;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data
{
    public abstract class PriceIdentifier : LearnerIdentifier
    {
        public string PriceEpisodeIdentifier { get; set; }

        public DateTime EpisodeStartDate { get; set; }

        public DateTime EpisodeEffectiveTNPStartDate { get; set; }

        public decimal TotalNegotiatedPrice { get; set; }
    }
}