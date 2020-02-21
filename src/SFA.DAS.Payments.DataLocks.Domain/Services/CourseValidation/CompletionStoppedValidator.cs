using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public interface ICompletionStoppedValidator
    {
        (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures) Validate(PriceEpisode ilrPriceEpisode, List<ApprenticeshipModel> apprenticeships, TransactionType transactionType);
         
        }

    public class CompletionStoppedValidator : ICompletionStoppedValidator
    {
        public (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures) Validate
            (PriceEpisode ilrPriceEpisode, List<ApprenticeshipModel> apprenticeships, TransactionType transactionType)
        {

            // Only deal with Transaction Type 2 & 3 (Completion and balancing)
            if (transactionType != TransactionType.Completion && transactionType != TransactionType.Balancing)
            {
                return (apprenticeships, new List<DataLockFailure>());
            }

            var matchedApprenticeships = apprenticeships
                .Where(a => (ilrPriceEpisode.ActualEndDate <= a.StopDate || a.Status != ApprenticeshipStatus.Stopped))
                .ToList();

            if (matchedApprenticeships.Any())
            {
                return (matchedApprenticeships, new List<DataLockFailure>());
            }
            
            var dataLockFailures = apprenticeships.Select(a => new DataLockFailure
            {
                ApprenticeshipId = a.Id,
                ApprenticeshipPriceEpisodeIds = a.ApprenticeshipPriceEpisodes
                    .Where(o => !o.Removed)
                    .Select(x => x.Id).ToList(),
                DataLockError = DataLockErrorCode.DLOCK_10
            }).ToList();

            return (new List<ApprenticeshipModel>(), dataLockFailures);
        }
    }
}
