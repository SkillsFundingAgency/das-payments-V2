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
                dataLockEvents.Add(payableEarningEvent);
            }

            if (onProgrammeEarning.invalidOnProgEarnings.Any())
            {
                var earningFailedDataLockEvent = mapper.Map<EarningFailedDataLockMatching>(earningEvent);
                earningFailedDataLockEvent.OnProgrammeEarnings = onProgrammeEarning.invalidOnProgEarnings;
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
                var validationResult = onProgrammePeriodsValidationProcessor.ValidatePeriods(earningEvent.Learner.Uln, earningEvent.PriceEpisodes, onProgrammeEarning, apprenticeshipsForUln, earningEvent.LearningAim, earningEvent.CollectionYear);

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
            var payableEarningEvent = mapper.Map<PayableEarningEvent>(earningEvent);

            payableEarningEvent.OnProgrammeEarnings = new List<OnProgrammeEarning>();
            nonPayableEarning.OnProgrammeEarnings = new List<OnProgrammeEarning>();

            foreach (var onProgrammeEarning in earningEvent.OnProgrammeEarnings)
            {
                var validPeriods = new List<EarningPeriod>();
                var invalidPeriods = new List<EarningPeriod>();

                foreach (var period in onProgrammeEarning.Periods)
                {
                    if (period.Amount == decimal.Zero)
                    {
                        validPeriods.Add(period);
                    }
                    else
                    {
                        period.DataLockFailures = new List<DataLockFailure>
                        {
                            new DataLockFailure
                            {
                                DataLockError = dataLockErrorCode
                            }
                        };

                        invalidPeriods.Add(period);
                    }
                }

                if (invalidPeriods.Any()) nonPayableEarning.OnProgrammeEarnings.Add(CreateOnProgrammeEarning(onProgrammeEarning, invalidPeriods));
                if (validPeriods.Any()) payableEarningEvent.OnProgrammeEarnings.Add(CreateOnProgrammeEarning(onProgrammeEarning, validPeriods));
            }

            if (nonPayableEarning.OnProgrammeEarnings.Any())dataLockEvents.Add(nonPayableEarning);
            if (payableEarningEvent.OnProgrammeEarnings.Any()) dataLockEvents.Add(payableEarningEvent);

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
    }
}
