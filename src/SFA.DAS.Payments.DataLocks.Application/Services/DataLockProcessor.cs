using AutoMapper;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class DataLockProcessor : IDataLockProcessor
    {
        private readonly IMapper mapper;
        private readonly ILearnerMatcher learnerMatcher;
        private readonly ICourseValidatorsProcessor processCourseValidator;

        public DataLockProcessor(IMapper mapper, ILearnerMatcher learnerMatcher, ICourseValidatorsProcessor processCourseValidator)
        {
            this.mapper = mapper;
            this.learnerMatcher = learnerMatcher;
            this.processCourseValidator = processCourseValidator;
        }

        public async Task<DataLockEvent> GetPaymentEvent(ApprenticeshipContractType1EarningEvent earningEvent, CancellationToken cancellationToken)
        {
            var learnerMatchResult = await learnerMatcher.MatchLearner(earningEvent.Learner.Uln).ConfigureAwait(false);
            if (learnerMatchResult.DataLockErrorCode.HasValue)
            {
                return CreateDataLockNonPayableEarningEvent(earningEvent, learnerMatchResult.DataLockErrorCode.Value);
            }

            var apprenticeshipsForUln = learnerMatchResult.Apprenticeships;
            var payableEarningEvent = mapper.Map<PayableEarningEvent>(earningEvent);

            var validationResults = FilterForValidationResults(payableEarningEvent, apprenticeshipsForUln);
            var apprenticeship = GetValidApprenticeship(validationResults, apprenticeshipsForUln);

            if (apprenticeship != null)
            {
                payableEarningEvent.AccountId = apprenticeship.AccountId;
                payableEarningEvent.Priority = apprenticeship.Priority;
            }
            else
            {
                if (payableEarningEvent.OnProgrammeEarnings.SelectMany(x => x.Periods).Any(p => p.Amount != decimal.Zero))
                {
                    throw new InvalidOperationException("There are no valid apprenticeship to complete DataLock");
                }
            }

            return payableEarningEvent;
        }

        private ApprenticeshipModel GetValidApprenticeship(List<ValidationResult> allPeriodValidationResults, List<ApprenticeshipModel> apprenticeshipsForUln)
        {
            return apprenticeshipsForUln
                .OrderByDescending(x => x.EstimatedStartDate)
                .FirstOrDefault(a => allPeriodValidationResults.All(x => x.ApprenticeshipId != a.Id));
        }

        private List<ValidationResult> FilterForValidationResults(PayableEarningEvent payableEarningEvent, List<ApprenticeshipModel> apprenticeshipsForUln)
        {
            var allPeriodValidationResults = new List<ValidationResult>();

            foreach (var onProgrammeEarning in payableEarningEvent.OnProgrammeEarnings)
            {
                var periodsValidationResults = ValidOnProgEarningPeriods(payableEarningEvent.Learner.Uln,
                    payableEarningEvent.PriceEpisodes, onProgrammeEarning, apprenticeshipsForUln);

                allPeriodValidationResults.AddRange(periodsValidationResults);

                var validPeriods = onProgrammeEarning.Periods
                    .Where(p => periodsValidationResults.All(x => x.Period != p.Period)).ToList();

                onProgrammeEarning.Periods = new ReadOnlyCollection<EarningPeriod>(validPeriods);
            }

            return allPeriodValidationResults;
        }

        private NonPayableEarningEvent CreateDataLockNonPayableEarningEvent(ApprenticeshipContractType1EarningEvent earningEvent, DataLockErrorCode dataLockErrorCode)
        {
            var nonPayableEarning = mapper.Map<NonPayableEarningEvent>(earningEvent);
            nonPayableEarning.Errors = new ReadOnlyCollection<DataLockErrorCode>(
                new[]
                {
                    dataLockErrorCode
                });
            return nonPayableEarning;
        }

        private List<ValidationResult> ValidOnProgEarningPeriods(long uln, List<PriceEpisode> priceEpisodes, OnProgrammeEarning onProgrammeEarning, List<ApprenticeshipModel> apprenticeships)
        {
            var onProgrammeEarningPeriods = onProgrammeEarning.Periods;
            var validationResults = new List<ValidationResult>();

            foreach (var period in onProgrammeEarningPeriods)
            {
                if (period.Amount == decimal.Zero) continue;

                var validationModel = new DataLockValidationModel
                {
                    Uln = uln,
                    EarningPeriod = period,
                    Apprenticeships = apprenticeships,
                    PriceEpisode = priceEpisodes.Single(o => o.Identifier.Equals(period.PriceEpisodeIdentifier, StringComparison.OrdinalIgnoreCase))
                };

                var periodValidationResults = processCourseValidator.ValidateCourse(validationModel);

                if (periodValidationResults != null && periodValidationResults.Any())
                {
                    validationResults.AddRange(periodValidationResults);
                }
            }

            return validationResults;
        }
    }
}
