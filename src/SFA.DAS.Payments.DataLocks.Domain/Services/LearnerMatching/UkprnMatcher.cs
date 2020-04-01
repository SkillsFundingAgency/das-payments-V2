using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching
{
    public class UkprnMatcher: IUkprnMatcher
    {
        public UkprnMatcher()
        {
        }

        public LearnerMatchResult MatchUkprn(long ukprn, List<ApprenticeshipModel> apprenticeships)
        {
            var validApprenticeships = apprenticeships.Where(apprenticeship => apprenticeship.Ukprn == ukprn).ToList();
            return new LearnerMatchResult
            {
                Apprenticeships = validApprenticeships,
                DataLockErrorCode = !validApprenticeships.Any() ? DataLockErrorCode.DLOCK_01 : (DataLockErrorCode?)null
            };
        }

    }
}
