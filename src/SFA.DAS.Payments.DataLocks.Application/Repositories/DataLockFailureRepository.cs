using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Model.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{
    public interface IDataLockFailureRepository
    {
        Task<List<DataLockFailureModel>> GetFailures(
            long ukprn,
            string learnerReferenceNumber,
            int frameworkCode,
            int pathwayCode,
            int programmeType,
            int standardCode,
            string learnAimRef,
            short academicYear
        );

        Task<List<DataLockFailureModel>> DeleteFailures(
            long ukprn,
            string learnerReferenceNumber,
            int frameworkCode,
            int pathwayCode,
            int programmeType,
            int standardCode,
            string learnAimRef,
            short academicYear,
            byte period,
            byte transactionType
        );
    }
}
