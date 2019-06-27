using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IApprenticeshipKeyService
    {
        string GenerateApprenticeshipKey(long ukprn, string learnerReferenceNumber, int frameworkCode, int pathwayCode,
            int programmeType, int standardCode, string learnAimRef, short academicYear, ContractType contractType);
        ApprenticeshipKey ParseApprenticeshipKey(string apprenticeshipKey);
    }
}
