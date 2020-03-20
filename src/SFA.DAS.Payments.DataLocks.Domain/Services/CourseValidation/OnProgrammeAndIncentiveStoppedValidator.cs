using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public interface IOnProgrammeAndIncentiveStoppedValidator
    {
        (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures) Validate(List<ApprenticeshipModel> apprenticeships, TransactionType transactionType, EarningPeriod earningPeriod, int academicYear);
    }

    public class OnProgrammeAndIncentiveStoppedValidator : IOnProgrammeAndIncentiveStoppedValidator
    {
        private readonly ICalculatePeriodStartAndEndDate calculatePeriodStartAndEndDate;

        public OnProgrammeAndIncentiveStoppedValidator(ICalculatePeriodStartAndEndDate calculatePeriodStartAndEndDate)
        {
            this.calculatePeriodStartAndEndDate = calculatePeriodStartAndEndDate;
        }

        public (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures)
            Validate(List<ApprenticeshipModel> apprenticeships, TransactionType transactionType, EarningPeriod earningPeriod, int academicYear)
        {

            // Only deal with Transaction Type 1 OnProgramme and incentives
            if (transactionType == TransactionType.Balancing || transactionType == TransactionType.Completion)
            {
                return (apprenticeships, new List<DataLockFailure>());
            }

            var periodDate = calculatePeriodStartAndEndDate.GetPeriodDate(earningPeriod.Period, academicYear);

            var matchedApprenticeships = apprenticeships
                .Where(a =>
                {
                    if (a.Status != ApprenticeshipStatus.Stopped)
                    {
                        return true;
                    }

                    if (a.StopDate >= periodDate.periodEndDate)
                    {
                        return true;
                    }

                    return false;
                })
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
