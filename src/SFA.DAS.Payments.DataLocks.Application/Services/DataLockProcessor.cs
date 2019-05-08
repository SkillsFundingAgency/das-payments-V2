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

        public async Task<DataLockEvent> GetPaymentEvent(ApprenticeshipContractType1EarningEvent earningEvent, CancellationToken cancellationToken)
        {
            var learnerMatchResult = await learnerMatcher.MatchLearner(earningEvent.Learner.Uln).ConfigureAwait(false);
            if (learnerMatchResult.DataLockErrorCode.HasValue)
            {
                return CreateDataLockNonPayableEarningEvent(earningEvent, learnerMatchResult.DataLockErrorCode.Value);
            }

            var apprenticeshipsForUln = learnerMatchResult.Apprenticeships;
            var payableEarningEvent = mapper.Map<PayableEarningEvent>(earningEvent);

            FilterPayableEarningPeriods(payableEarningEvent, apprenticeshipsForUln);
            return payableEarningEvent;
        }

        //TODO: Signature needs to change to cope with non-payable earnings - PV2-835
        private void FilterPayableEarningPeriods(PayableEarningEvent payableEarningEvent, List<ApprenticeshipModel> apprenticeshipsForUln)
        {
            foreach (var onProgrammeEarning in payableEarningEvent.OnProgrammeEarnings)
            {
                var validationResult = onProgrammePeriodsValidationProcessor.ValidatePeriods(
                    payableEarningEvent.Learner.Uln, payableEarningEvent.PriceEpisodes, onProgrammeEarning, apprenticeshipsForUln);
                
                onProgrammeEarning.Periods =
                    validationResult.ValidPeriods.Select(valid => valid.Period).ToList().AsReadOnly();
            }
        }

        //TODO: Create real NonPayableEarningEvent - PV2-835
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

    }
}
