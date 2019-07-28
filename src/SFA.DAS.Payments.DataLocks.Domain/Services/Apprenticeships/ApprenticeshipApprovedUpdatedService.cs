using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships
{
    public class ApprenticeshipApprovedUpdatedService : ApprenticeshipUpdatedService<UpdatedApprenticeshipApprovedModel>, IApprenticeshipApprovedUpdatedService
    {
        public ApprenticeshipApprovedUpdatedService(IApprenticeshipRepository repository) : base(repository)
        {

        }

        protected override void HandleUpdated(ApprenticeshipModel current, UpdatedApprenticeshipApprovedModel updated)
        {
            current.AgreedOnDate = updated.AgreedOnDate;
            current.Uln = updated.Uln;
            current.EstimatedStartDate = updated.EstimatedStartDate;
            current.EstimatedEndDate = updated.EstimatedEndDate;
            current.StandardCode = updated.StandardCode;
            current.ProgrammeType = updated.ProgrammeType;
            current.FrameworkCode = updated.FrameworkCode;
            current.PathwayCode = updated.PathwayCode;
        }
    }
}