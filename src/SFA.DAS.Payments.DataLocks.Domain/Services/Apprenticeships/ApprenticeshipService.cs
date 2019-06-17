using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships
{
    public interface IApprenticeshipService
    {
        Task NewApprenticeship(ApprenticeshipModel apprenticeship);
    }

    public class ApprenticeshipService: IApprenticeshipService
    {
        public Task NewApprenticeship(ApprenticeshipModel apprenticeship)
        {
            throw new System.NotImplementedException();
        }
    }
}