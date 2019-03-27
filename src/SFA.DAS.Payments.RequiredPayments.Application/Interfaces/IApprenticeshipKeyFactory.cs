using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IApprenticeshipKeyFactory
    {
        ApprenticeshipKey GetCurrentKey();
    }
}