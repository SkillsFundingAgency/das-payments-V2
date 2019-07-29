using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships
{
    public class ApprenticeshipDataLockTriageService: ApprenticeshipUpdatedService<UpdatedApprenticeshipDataLockTriageModel>, IApprenticeshipDataLockTriageService
    {
        public ApprenticeshipDataLockTriageService(IApprenticeshipRepository repository) : base(repository)
        {
        }

        protected override void HandleUpdated(ApprenticeshipModel current, UpdatedApprenticeshipDataLockTriageModel updated)
        {
            current.AgreedOnDate = updated.AgreedOnDate;
            current.StandardCode = updated.StandardCode;
            current.ProgrammeType = updated.ProgrammeType;
            current.FrameworkCode = updated.FrameworkCode;
            current.PathwayCode = updated.PathwayCode;
        }
    }
}
