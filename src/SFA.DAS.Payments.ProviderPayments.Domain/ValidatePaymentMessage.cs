using SFA.DAS.Payments.ProviderPayments.Domain.Models;

namespace SFA.DAS.Payments.ProviderPayments.Domain
{
    public interface IValidateIlrSubmission
    {
        bool IsLatestIlrPayment(IlrSubmissionValidationRequest request);
    }

    public class ValidateIlrSubmission : IValidateIlrSubmission
    {
        public bool IsLatestIlrPayment(IlrSubmissionValidationRequest request)
        {
            return request.CurrentIlr == null ||
                    (request.IncomingPaymentUkprn == request.CurrentIlr.Ukprn &&
                    request.IncomingPaymentSubmissionDate.CompareTo(request.CurrentIlr.IlrSubmissionDateTime) >= 0);
        }
    }
}
