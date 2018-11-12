using System;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.ProviderPayments.Domain.Models
{
    public class IlrSubmissionValidationRequest
    {
        public IlrSubmittedEvent CurrentIlr { get; set; }
        public long IncomingPaymentUkprn { get; set; }
        public DateTime IncomingPaymentSubmissionDate { get; set; }

    }
}
