using SFA.DAS.Payments.ProviderPayments.Domain.Models;

namespace SFA.DAS.Payments.ProviderPayments.Domain
{
    public interface IValidateIlrSubmission
    {
        bool IsLatestIlrPayment(IlrSubmissionValidationRequest request);
    }
}