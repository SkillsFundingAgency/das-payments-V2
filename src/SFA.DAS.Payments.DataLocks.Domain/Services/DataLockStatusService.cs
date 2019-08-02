using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class DataLockStatusService : IDataLockStatusService
    {
        public DataLockStatusChange GetStatusChange(List<DataLockFailure> oldFailures, List<DataLockFailure> newFailures)
        {
            if (oldFailures == null && newFailures == null)
                return DataLockStatusChange.NoChange;

            if (oldFailures == null)
                return DataLockStatusChange.ChangedToFailed;

            if (newFailures == null)
                return DataLockStatusChange.ChangedToPassed;

            if (oldFailures.Count != newFailures.Count)
                return DataLockStatusChange.FailureChanged;

            var orderedOld = OrderFailures(oldFailures);
            var orderedNew = OrderFailures(newFailures);

            for (var i = 0; i < orderedNew.Count; i++)
            {
                var oldFailure = orderedOld[i];
                var newFailure = orderedNew[i];

                if (oldFailure.ApprenticeshipId != newFailure.ApprenticeshipId)
                    return DataLockStatusChange.FailureChanged;

                if (oldFailure.DataLockError != newFailure.DataLockError)
                    return DataLockStatusChange.FailureChanged;

                var oldEpisodes = oldFailure.ApprenticeshipPriceEpisodeIds ?? new List<long>();
                var newEpisodes = newFailure.ApprenticeshipPriceEpisodeIds ?? new List<long>();

                if (oldEpisodes.Count != newEpisodes.Count)
                    return DataLockStatusChange.FailureChanged;

                for (var k = 0; k < oldEpisodes.Count; k++)
                {
                    if (!oldEpisodes.Contains(newEpisodes[k]) || !newEpisodes.Contains(oldEpisodes[k]))
                        return DataLockStatusChange.FailureChanged;
                }
            }

            return DataLockStatusChange.NoChange;
        }

        private static List<DataLockFailure> OrderFailures(List<DataLockFailure> currentFailures)
        {
            return currentFailures.OrderBy(f => f.DataLockError)
                .ThenBy(f => f.ApprenticeshipId)
                .ThenBy(f => f.ApprenticeshipPriceEpisodeIds == null ? string.Empty : string.Join("-", f.ApprenticeshipPriceEpisodeIds.OrderBy(p => p)))
                .ToList();
        }
    }
}