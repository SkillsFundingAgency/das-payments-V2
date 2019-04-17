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
using SFA.DAS.Payments.Messages.Core.Events;

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
            var apprenticeship = apprenticeshipsForUln.First();
            payableEarningEvent.AccountId = apprenticeship.AccountId;
            payableEarningEvent.Priority = apprenticeship.Priority;

            FilterToValidEarningPeriods(payableEarningEvent, apprenticeshipsForUln);

            return payableEarningEvent;
        }

        private void FilterToValidEarningPeriods(PayableEarningEvent payableEarningEvent, List<ApprenticeshipModel> apprenticeshipsForUln)
        {
            foreach (var onProgrammeEarning in payableEarningEvent.OnProgrammeEarnings)
            {
                var periodsValidationResults = ValidOnProgEarningPeriods(payableEarningEvent.Learner.Uln,
                    payableEarningEvent.PriceEpisodes, onProgrammeEarning, apprenticeshipsForUln);

                var validPeriods = onProgrammeEarning.Periods
                    .Where(p => periodsValidationResults.All(x => x.Period != p.Period)).ToList();

                onProgrammeEarning.Periods = new ReadOnlyCollection<EarningPeriod>(validPeriods);
            }
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
