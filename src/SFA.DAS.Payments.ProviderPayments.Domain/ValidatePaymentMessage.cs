using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using System;

namespace SFA.DAS.Payments.ProviderPayments.Domain
{
    public class ValidatePaymentMessage : IValidatePaymentMessage
    {
        public bool IsLatestIlrPayment(PaymentMessageValidationRequest request)
        {
            return request.CurrentIlr == null ||
                   (request.IncomingPaymentJobId.Equals(request.CurrentIlr.JobId, StringComparison.OrdinalIgnoreCase) &&
                    request.IncomingPaymentUkprn == request.CurrentIlr.Ukprn) &&
                    request.IncomingPaymentSubmissionDate.CompareTo(request.CurrentIlr.SubmissionDate) == 0;
        }

    }
}
