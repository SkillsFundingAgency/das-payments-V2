using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships
{
    public class ApprenticeshipPauseService : ApprenticeshipUpdatedService<UpdatedApprenticeshipPausedModel>, IApprenticeshipPauseService
    {
        public ApprenticeshipPauseService(IApprenticeshipRepository repository) : base(repository)
        {
        }

        protected override void HandleUpdated(ApprenticeshipModel current, UpdatedApprenticeshipPausedModel updated)
        {
            current.Status = ApprenticeshipStatus.Paused;
        }

        protected override async Task UpdateHistoryTables( UpdatedApprenticeshipPausedModel updated)
        {
            var newPauseModel = new ApprenticeshipPauseModel
            {
                ApprenticeshipId = updated.ApprenticeshipId,
                PauseDate = updated.PauseDate
            };

           await  repository.AddApprenticeshipPause(newPauseModel);
        }
    }
}
