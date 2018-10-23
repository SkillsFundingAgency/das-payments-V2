using System;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.ProviderPayments.Domain.Models
{
    public class PaymentMessageValidationRequest
    {
        public IlrSubmittedEvent CurrentIlr { get; set; }

        public string IncomingPaymentJobId { get; set; }
        public long IncomingPaymentUkprn { get; set; }
        public DateTime IncomingPaymentSubmissionDate { get; set; }

    }
}
