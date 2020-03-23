using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships
{
    public class ApprenticeshipResumedService : ApprenticeshipUpdatedService<UpdatedApprenticeshipResumedModel>, IApprenticeshipResumedService
    {
        public ApprenticeshipResumedService(IApprenticeshipRepository repository) : base(repository)
        {
        }

        protected override void HandleUpdated(ApprenticeshipModel current, UpdatedApprenticeshipResumedModel updated)
        {
            current.Status = ApprenticeshipStatus.Active;
        }

        protected override async Task UpdateHistoryTables( UpdatedApprenticeshipResumedModel updated)
        {
            var currentPauseModel = await repository
                .GetCurrentApprenticeshipPausedModel(updated.ApprenticeshipId)
                .ConfigureAwait(false);

            if(currentPauseModel ==null)
                throw new InvalidOperationException($"Can't find any currently paused Apprenticeship with Id: {updated.ApprenticeshipId}");

            currentPauseModel.ResumeDate = updated.ResumedDate;

           await  repository.UpdateCurrentlyPausedApprenticeship(currentPauseModel);
        }
    }
}
