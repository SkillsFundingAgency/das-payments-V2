using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class ApprenticeshipPriceEpisodeModel
    {
        public long Id { get; set; }
        public long ApprenticeshipId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public decimal Cost { get; set; }
        public bool Removed { get; set; }
        public ApprenticeshipModel Apprenticeship { get; set; }
    }
}