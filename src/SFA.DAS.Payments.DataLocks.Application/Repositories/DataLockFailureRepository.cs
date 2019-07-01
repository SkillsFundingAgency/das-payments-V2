using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core.Entities;

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
            short academicYear,
            ContractType contractType,
            byte period
        );

        Task<List<DataLockFailureModel>> DeleteFailures(List<DataLockFailureModel> failures);
    }
}
