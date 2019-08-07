using AutoMapper;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
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
        private readonly IEarningPeriodsValidationProcessor earningPeriodsValidationProcessor;

        public DataLockProcessor(IMapper mapper, ILearnerMatcher learnerMatcher, IEarningPeriodsValidationProcessor earningPeriodsValidationProcessor)
        {
            this.mapper = mapper;
            this.learnerMatcher = learnerMatcher;
            this.earningPeriodsValidationProcessor = earningPeriodsValidationProcessor ?? throw new ArgumentNullException(nameof(earningPeriodsValidationProcessor));
        }

        public async Task<List<DataLockEvent>> GetPaymentEvents(ApprenticeshipContractType1EarningEvent earningEvent, CancellationToken cancellationToken)
        {
            var dataLockEvents = new List<DataLockEvent>();

            var learnerMatchResult = await learnerMatcher.MatchLearner(earningEvent.Ukprn, earningEvent.Learner.Uln).ConfigureAwait(false);
            if (learnerMatchResult.DataLockErrorCode.HasValue)
            {
                dataLockEvents = CreateDataLockEvents(earningEvent, learnerMatchResult.DataLockErrorCode.Value);
                return dataLockEvents;
            }

            var apprenticeshipsForUln = learnerMatchResult.Apprenticeships;
            var onProgrammeEarning = GetOnProgrammeEarnings(earningEvent, apprenticeshipsForUln);
            var incentiveEarnings = GetIncentiveEarnings(earningEvent, apprenticeshipsForUln);

            if (onProgrammeEarning.validOnProgEarnings.Any())
            {
                var payableEarningEvent = mapper.Map<PayableEarningEvent>(earningEvent);
                payableEarningEvent.OnProgrammeEarnings = onProgrammeEarning.validOnProgEarnings;
                payableEarningEvent.IncentiveEarnings = incentiveEarnings.validIncentiveEarnings;
                dataLockEvents.Add(payableEarningEvent);
            }

            if (onProgrammeEarning.invalidOnProgEarnings.Any())
            {
                var earningFailedDataLockEvent = mapper.Map<EarningFailedDataLockMatching>(earningEvent);
                earningFailedDataLockEvent.OnProgrammeEarnings = onProgrammeEarning.invalidOnProgEarnings;
                earningFailedDataLockEvent.IncentiveEarnings = incentiveEarnings.invalidIncentiveEarning;
                dataLockEvents.Add(earningFailedDataLockEvent);
            }

            return dataLockEvents;
        }

        private (List<OnProgrammeEarning> validOnProgEarnings, List<OnProgrammeEarning> invalidOnProgEarnings) GetOnProgrammeEarnings(
            ApprenticeshipContractTypeEarningsEvent earningEvent, 
            List<ApprenticeshipModel> apprenticeshipsForUln)
        {
            var validOnProgEarnings = new List<OnProgrammeEarning>();
            var invalidOnProgEarnings = new List<OnProgrammeEarning>();

            foreach (var onProgrammeEarning in earningEvent.OnProgrammeEarnings)
            {
                var validationResult = earningPeriodsValidationProcessor
                    .ValidatePeriods(
                        earningEvent.Ukprn,
                        earningEvent.Learner.Uln, 
                        earningEvent.PriceEpisodes,
                        onProgrammeEarning.Periods.ToList(),
                        (TransactionType)onProgrammeEarning.Type,
                        apprenticeshipsForUln, 
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

        private (List<IncentiveEarning> validIncentiveEarnings, List<IncentiveEarning> invalidIncentiveEarning) GetIncentiveEarnings(
                ApprenticeshipContractTypeEarningsEvent earningEvent, 
                List<ApprenticeshipModel> apprenticeshipsForUln)
        {
            var validIncentiveEarnings = new List<IncentiveEarning>();
            var invalidIncentiveEarning = new List<IncentiveEarning>();

            foreach (var incentiveEarning in earningEvent.IncentiveEarnings)
            {
                var validationResult = earningPeriodsValidationProcessor
                    .ValidatePeriods(
                        earningEvent.Ukprn,
                        earningEvent.Learner.Uln,
                        earningEvent.PriceEpisodes,
                        incentiveEarning.Periods.ToList(),
                        (TransactionType)incentiveEarning.Type,
                        apprenticeshipsForUln,
                        earningEvent.LearningAim,
                        earningEvent.CollectionYear);

                if (validationResult.ValidPeriods.Any())
                {
                    validIncentiveEarnings.Add(CreateIncentiveEarning(incentiveEarning, validationResult.ValidPeriods));
                }

                if (validationResult.InValidPeriods.Any())
                {
                    invalidIncentiveEarning.Add(CreateIncentiveEarning(incentiveEarning, validationResult.InValidPeriods));
                }
            }

            return (validIncentiveEarnings, invalidIncentiveEarning);
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

        private static IncentiveEarning CreateIncentiveEarning(IncentiveEarning incentiveEarning, List<EarningPeriod> periods)
        {
            return new IncentiveEarning
            {
                CensusDate = incentiveEarning.CensusDate,
                Type = incentiveEarning.Type,
                Periods = periods.AsReadOnly()
            };
        }


    }
}
