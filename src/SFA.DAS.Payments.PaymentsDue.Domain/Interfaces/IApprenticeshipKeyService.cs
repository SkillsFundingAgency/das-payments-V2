using SFA.DAS.Payments.PaymentsDue.Domain.Enums;

namespace SFA.DAS.Payments.PaymentsDue.Domain.Interfaces
{
    public interface IApprenticeshipKeyService
    {
        string GenerateKey(long ukprn, string learnerReferenceNumber, int frameworkCode, int pathwayCode, ProgrammeType programmeType, int standardCode, string learnAimRef);
    }
}
