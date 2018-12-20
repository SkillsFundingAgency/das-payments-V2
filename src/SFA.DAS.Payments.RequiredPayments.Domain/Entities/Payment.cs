using System;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public CalendarPeriod DeliveryPeriod { get; set; }

        public CalendarPeriod CollectionPeriod { get; set; }
    }
}
