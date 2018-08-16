using SFA.DAS.Payments.RequiredPayments.Domain.Enums;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Interfaces
{
    public interface IApprenticeshipKeyService
    {
        string GenerateKey(long ukprn, string learnerReferenceNumber, int frameworkCode, int pathwayCode, ProgrammeType programmeType, int standardCode, string learnAimRef);
    }
}
