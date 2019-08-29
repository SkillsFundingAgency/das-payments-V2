using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships
{
    public class ApprenticeshipStoppedService : ApprenticeshipUpdatedService<UpdatedApprenticeshipStoppedModel>, IApprenticeshipStoppedService
    {
        public ApprenticeshipStoppedService(IApprenticeshipRepository repository) : base(repository)
        {
        }

        protected override void HandleUpdated(ApprenticeshipModel current, UpdatedApprenticeshipStoppedModel updated)
        {
            current.Status = ApprenticeshipStatus.Stopped;
            current.StopDate = updated.StopDate;

            foreach (var currentApprenticeshipPriceEpisode in current.ApprenticeshipPriceEpisodes)
            {
                if (!currentApprenticeshipPriceEpisode.Removed)
                {
                    currentApprenticeshipPriceEpisode.EndDate = updated.StopDate;
                }
            }
        }

    }
}
