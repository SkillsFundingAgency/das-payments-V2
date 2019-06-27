using AutoMapper;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class DataLockProcessor : IDataLockProcessor
    {
        private readonly IMapper mapper;
        private readonly ILearnerMatcher learnerMatcher;
        private readonly IOnProgrammePeriodsValidationProcessor onProgrammePeriodsValidationProcessor;

        public DataLockProcessor(IMapper mapper, ILearnerMatcher learnerMatcher, IOnProgrammePeriodsValidationProcessor onProgrammePeriodsValidationProcessor)
        {
            this.mapper = mapper;
            this.learnerMatcher = learnerMatcher;
            this.onProgrammePeriodsValidationProcessor = onProgrammePeriodsValidationProcessor ?? throw new ArgumentNullException(nameof(onProgrammePeriodsValidationProcessor));
        }

        public async Task<List<DataLockEvent>> GetPaymentEvents(ApprenticeshipContractType1EarningEvent earningEvent, CancellationToken cancellationToken)
        {
            var dataLockEvents = new List<DataLockEvent>();

            var learnerMatchResult = await learnerMatcher.MatchLearner(earningEvent.Learner.Uln).ConfigureAwait(false);
            if (learnerMatchResult.DataLockErrorCode.HasValue)
            {
                dataLockEvents = CreateDataLockEvents(earningEvent, learnerMatchResult.DataLockErrorCode.Value);
                return dataLockEvents;
            }

            var apprenticeshipsForUln = learnerMatchResult.Apprenticeships;
            var onProgrammeEarning = GetOnProgrammeEarnings(earningEvent, apprenticeshipsForUln);

            if (onProgrammeEarning.validOnProgEarnings.Any())
            {
                var payableEarningEvent = mapper.Map<PayableEarningEvent>(earningEvent);
                payableEarningEvent.OnProgrammeEarnings = onProgrammeEarning.validOnProgEarnings;
                payableEarningEvent.IncentiveEarnings = GetMatchingOnProgEarningPeriodIncentives(earningEvent.IncentiveEarnings, onProgrammeEarning.validOnProgEarnings);
                dataLockEvents.Add(payableEarningEvent);
            }

            if (onProgrammeEarning.invalidOnProgEarnings.Any())
            {
                var earningFailedDataLockEvent = mapper.Map<EarningFailedDataLockMatching>(earningEvent);
                earningFailedDataLockEvent.OnProgrammeEarnings = onProgrammeEarning.invalidOnProgEarnings;
                earningFailedDataLockEvent.IncentiveEarnings = GetMatchingOnProgEarningPeriodIncentives(earningEvent.IncentiveEarnings, onProgrammeEarning.invalidOnProgEarnings);
                dataLockEvents.Add(earningFailedDataLockEvent);
            }

            return dataLockEvents;
        }

        private (List<OnProgrammeEarning> validOnProgEarnings, List<OnProgrammeEarning> invalidOnProgEarnings) GetOnProgrammeEarnings(ApprenticeshipContractTypeEarningsEvent earningEvent, List<ApprenticeshipModel> apprenticeshipsForUln)
        {
            var validOnProgEarnings = new List<OnProgrammeEarning>();
            var invalidOnProgEarnings = new List<OnProgrammeEarning>();

            foreach (var onProgrammeEarning in earningEvent.OnProgrammeEarnings)
            {
                var validationResult = onProgrammePeriodsValidationProcessor
                    .ValidatePeriods(earningEvent.Learner.Uln, 
                        earningEvent.PriceEpisodes,
                        onProgrammeEarning, apprenticeshipsForUln, 
                        earningEvent.LearningAim,
                        earningEvent.CollectionYear);

                if (validationResult.ValidPeriods.Any())
                {
                    validOnProgEarnings.Add(CreateOnProgrammeEarning(onProgrammeEarning, validationResult.ValidPeriods));
                }

                if (validationResult.InValidPeriods.Any())
                {
                    invalidOnProgEarnings.Add(CreateOnProgrammeEarning(onProgrammeEarning, validationResult.InValidPeriods));
                }
            }

            return (validOnProgEarnings, invalidOnProgEarnings);
        }

        private List<DataLockEvent> CreateDataLockEvents(ApprenticeshipContractType1EarningEvent earningEvent, DataLockErrorCode dataLockErrorCode)
        {
            var dataLockEvents = new List<DataLockEvent>();
            var nonPayableEarning = mapper.Map<EarningFailedDataLockMatching>(earningEvent);

            foreach (var onProgrammeEarning in nonPayableEarning.OnProgrammeEarnings)
            {
                var validPeriods = new List<EarningPeriod>();
                var invalidPeriods = new List<EarningPeriod>();

                foreach (var period in onProgrammeEarning.Periods)
                {
                    period.DataLockFailures = new List<DataLockFailure>
                        {
                            new DataLockFailure
                            {
                                DataLockError = dataLockErrorCode
                            }
                        };
                }
            }

            foreach (var incentiveEarning in nonPayableEarning.IncentiveEarnings)
            {
                foreach (var period in incentiveEarning.Periods)
                {
                    period.DataLockFailures = new List<DataLockFailure>
                    {
                        new DataLockFailure
                        {
                            DataLockError = dataLockErrorCode
                        }
                    };
                }
            }

            if (nonPayableEarning.OnProgrammeEarnings.Any() || nonPayableEarning.IncentiveEarnings.Any())
            {
                dataLockEvents.Add(nonPayableEarning);
            }

            return dataLockEvents;
        }

        private static OnProgrammeEarning CreateOnProgrammeEarning(OnProgrammeEarning onProgrammeEarning, List<EarningPeriod> periods)
        {
            return new OnProgrammeEarning
            {
                CensusDate = onProgrammeEarning.CensusDate,
                Type = onProgrammeEarning.Type,
                Periods = periods.AsReadOnly()
            };
        }

        private List<IncentiveEarning> GetMatchingOnProgEarningPeriodIncentives(List<IncentiveEarning> incentiveEarnings, List<OnProgrammeEarning> onProgEarning)
        {
            if (incentiveEarnings == null) return new List<IncentiveEarning>();

            var matchedIncentiveEarnings = new List<IncentiveEarning>();
            var relevantPeriods = onProgEarning.SelectMany(p => p.Periods).ToList();

            foreach (var incentiveEarning in incentiveEarnings)
            {
                var matchingIncentivePeriods = incentiveEarning.Periods
                    .Where(p => relevantPeriods.Any(o => o.Period == p.Period))
                    .ToList();

                if (!matchingIncentivePeriods.Any()) continue;

                foreach (var incentivePeriod in matchingIncentivePeriods)
                {
                    var dataLockErrors = relevantPeriods
                        .Where(x => x.DataLockFailures != null)
                        .SelectMany(x => x.DataLockFailures)
                        .ToList();

                    if (!dataLockErrors.Any()) continue;

                    incentivePeriod.DataLockFailures = new List<DataLockFailure>();
                    incentivePeriod.DataLockFailures.AddRange(dataLockErrors);
                }

                matchedIncentiveEarnings.Add(new IncentiveEarning
                {
                    Periods = matchingIncentivePeriods.AsReadOnly(),
                    Type = incentiveEarning.Type,
                    CensusDate = incentiveEarning.CensusDate
                });
            }

            return matchedIncentiveEarnings;
        }

    }
}
