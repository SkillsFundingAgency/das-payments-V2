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
                var earningFailedDataLockEvent = CreateFailedDataLockMatchingEarningEvent(earningEvent, learnerMatchResult.DataLockErrorCode.Value);
                dataLockEvents.Add(earningFailedDataLockEvent);
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
                var validationResult = onProgrammePeriodsValidationProcessor.ValidatePeriods(earningEvent.Learner.Uln, earningEvent.PriceEpisodes, onProgrammeEarning, apprenticeshipsForUln, earningEvent.LearningAim);

                if (validationResult.ValidPeriods.Any())
                {
                    validOnProgEarnings.Add(new OnProgrammeEarning
                    {
                        CensusDate = onProgrammeEarning.CensusDate,
                        Type = onProgrammeEarning.Type,
                        Periods = validationResult.ValidPeriods.AsReadOnly()
                    });
                }

                if (validationResult.InValidPeriods.Any())
                {
                    invalidOnProgEarnings.Add(new OnProgrammeEarning
                    {
                        CensusDate = onProgrammeEarning.CensusDate,
                        Type = onProgrammeEarning.Type,
                        Periods = validationResult.InValidPeriods.AsReadOnly()
                    });
                }
            }

            return (validOnProgEarnings, invalidOnProgEarnings);
        }

        private EarningFailedDataLockMatching CreateFailedDataLockMatchingEarningEvent(ApprenticeshipContractType1EarningEvent earningEvent, DataLockErrorCode dataLockErrorCode)
        {
            var nonPayableEarning = mapper.Map<EarningFailedDataLockMatching>(earningEvent);
            foreach (var onProgrammeEarning in nonPayableEarning.OnProgrammeEarnings)
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
            return nonPayableEarning;
        }

    }
}
