using SFA.DAS.Payments.ProviderPayments.Domain.Models;

namespace SFA.DAS.Payments.ProviderPayments.Domain
{
    public class ValidateIlrSubmission : IValidateIlrSubmission
    {
        public bool IsLatestIlrPayment(IlrSubmissionValidationRequest request)
        {
            return request.CurrentIlr == null ||
                    (request.IncomingPaymentUkprn == request.CurrentIlr.Ukprn &&
                    request.IncomingPaymentSubmissionDate.CompareTo(request.CurrentIlr.IlrSubmissionDateTime) >= 0);
        }

        public bool IsNewIlrSubmission(IlrSubmissionValidationRequest request)
        {
            return request.CurrentIlr == null ||
                   request.IncomingPaymentUkprn != request.CurrentIlr.Ukprn ||
                   request.IncomingPaymentSubmissionDate.CompareTo(request.CurrentIlr.IlrSubmissionDateTime) != 0;
        }
    }
}
