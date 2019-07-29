using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public interface IEarningPeriodsValidationProcessor
    {
        (List<EarningPeriod> ValidPeriods, List<EarningPeriod> InValidPeriods) ValidatePeriods(long uln,
            List<PriceEpisode> priceEpisodes,
            List<EarningPeriod> periods,
            TransactionType transactionType,
            List<ApprenticeshipModel> apprenticeships,
            LearningAim aim,
            int academicYear);
    }
}