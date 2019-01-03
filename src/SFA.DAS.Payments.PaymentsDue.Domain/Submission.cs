using System;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PaymentsDue.Domain
{
    public class Submission
    {
        public long Ukprn { get; set; }
        public long JobId { get; set; }
        public DateTime IlrSubmissionDate { get; set; }
        public CalendarPeriod CollectionPeriod { get; set; }
    }
}
