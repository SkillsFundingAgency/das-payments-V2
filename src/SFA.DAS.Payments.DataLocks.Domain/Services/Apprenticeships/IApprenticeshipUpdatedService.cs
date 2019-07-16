using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships
{
    public interface IApprenticeshipUpdatedService<in T> where T : BaseUpdatedApprenticeshipModel
    {
        Task<ApprenticeshipModel> UpdateApprenticeship(T updatedApprenticeship);
    }

    public interface IApprenticeshipApprovedUpdatedService : IApprenticeshipUpdatedService<UpdatedApprenticeshipApprovedModel>
    {
    }

    public interface IApprenticeshipDataLockTriageService : IApprenticeshipUpdatedService<UpdatedApprenticeshipDataLockTriageModel>
    {
    }

    public interface IApprenticeshipStoppedService : IApprenticeshipUpdatedService<UpdatedApprenticeshipStoppedModel>
    {
    }
}