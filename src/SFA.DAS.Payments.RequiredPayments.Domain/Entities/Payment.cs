using System;
using System.Globalization;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public NamedCalendarPeriod DeliveryPeriod { get; set; }

        public NamedCalendarPeriod CollectionPeriod { get; set; }
    }
}
