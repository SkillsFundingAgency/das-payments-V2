using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching
{
    public interface IUkprnMatcher
    {
        LearnerMatchResult MatchUkprn(long ukprn, List<ApprenticeshipModel> apprenticeships);
    }
}