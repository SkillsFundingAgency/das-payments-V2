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
using SFA.DAS.Payments.Messages.Core.Events;
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
                payableEarningEvent.EventId = Guid.NewGuid();
                dataLockEvents.Add(payableEarningEvent);
            }

            if (onProgrammeEarning.invalidOnProgEarnings.Any())
            {
                var earningFailedDataLockEvent = mapper.Map<EarningFailedDataLockMatching>(earningEvent);
                earningFailedDataLockEvent.OnProgrammeEarnings = onProgrammeEarning.invalidOnProgEarnings;
                earningFailedDataLockEvent.IncentiveEarnings = incentiveEarnings.invalidIncentiveEarning;
                earningFailedDataLockEvent.EventId = Guid.NewGuid();
                dataLockEvents.Add(earningFailedDataLockEvent);
            }

            return dataLockEvents;
        }

        public async Task<List<FunctionalSkillDataLockEvent>> GetFunctionalSkillPaymentEvents(
            Act1FunctionalSkillEarningsEvent earningEvent, CancellationToken cancellationToken)
        {
            var dataLockEvents = new List<FunctionalSkillDataLockEvent>();

            var learnerMatchResult = await learnerMatcher.MatchLearner(earningEvent.Ukprn, earningEvent.Learner.Uln).ConfigureAwait(false);
            if (learnerMatchResult.DataLockErrorCode.HasValue)
            {
                dataLockEvents = CreateDataLockEvents(earningEvent, learnerMatchResult.DataLockErrorCode.Value);
                return dataLockEvents;
            }

            var apprenticeshipsForUln = learnerMatchResult.Apprenticeships;
            var functionalSkillEarnings = GetFunctionalSkillEarnings(earningEvent, apprenticeshipsForUln);

            if (functionalSkillEarnings.validEarnings.Any())
            {
                var payableEarningEvent = mapper.Map<PayableFunctionalSkillEarningEvent>(earningEvent);
                payableEarningEvent.Earnings = functionalSkillEarnings.validEarnings.AsReadOnly();
                payableEarningEvent.EventId = Guid.NewGuid();
                dataLockEvents.Add(payableEarningEvent);
            }

            if (functionalSkillEarnings.invalidEarnings.Any())
            {
                var earningFailedDataLockEvent = mapper.Map<FunctionalSkillEarningFailedDataLockMatching>(earningEvent);
                earningFailedDataLockEvent.Earnings = functionalSkillEarnings.invalidEarnings.AsReadOnly();
                earningFailedDataLockEvent.EventId = Guid.NewGuid();
                dataLockEvents.Add(earningFailedDataLockEvent);
            }

            return dataLockEvents;
        }

        private List<FunctionalSkillDataLockEvent> CreateDataLockEvents(IPaymentsEvent earningEvent, DataLockErrorCode dataLockErrorCode)
        {
            var dataLockEvents = new List<FunctionalSkillDataLockEvent>();
            var nonPayableEarning = mapper.Map<FunctionalSkillEarningFailedDataLockMatching>(earningEvent);

            foreach (var onProgrammeEarning in nonPayableEarning.Earnings)
            {
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

            if (nonPayableEarning.Earnings.Any())
            {
                dataLockEvents.Add(nonPayableEarning);
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

        private (List<FunctionalSkillEarning> validEarnings, List<FunctionalSkillEarning> invalidEarnings) GetFunctionalSkillEarnings(
            Act1FunctionalSkillEarningsEvent earningEvent,
            List<ApprenticeshipModel> apprenticeshipsForUln)
        {
            var validEarnings = new List<FunctionalSkillEarning>();
            var invalidEarnings = new List<FunctionalSkillEarning>();

            foreach (var functionalSkillEarning in earningEvent.Earnings)
            {
                var validationResult = earningPeriodsValidationProcessor
                    .ValidateFunctionalSkillPeriods(earningEvent.Ukprn,
                        earningEvent.Learner.Uln,
                        earningEvent.PriceEpisodes,
                        functionalSkillEarning.Periods.ToList(),
                        (TransactionType)functionalSkillEarning.Type,
                        apprenticeshipsForUln,
                        earningEvent.LearningAim,
                        earningEvent.CollectionYear);

                if (validationResult.ValidPeriods.Any())
                {
                    validEarnings.Add(CreateFunctionalSkillEarning(functionalSkillEarning, validationResult.ValidPeriods));
                }

                if (validationResult.InValidPeriods.Any())
                {
                    invalidEarnings.Add(CreateFunctionalSkillEarning(functionalSkillEarning, validationResult.InValidPeriods));
                }
            }

            return (validEarnings, invalidEarnings);
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

        private static FunctionalSkillEarning CreateFunctionalSkillEarning(FunctionalSkillEarning functionalSkillEarning, List<EarningPeriod> periods)
        {
            return new FunctionalSkillEarning
            {
                Type = functionalSkillEarning.Type,
                Periods = periods.AsReadOnly()
            };
        }
    }
}
