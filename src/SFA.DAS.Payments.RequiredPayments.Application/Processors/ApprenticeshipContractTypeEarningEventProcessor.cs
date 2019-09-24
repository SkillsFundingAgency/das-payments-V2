using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Domain;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public abstract class ApprenticeshipContractTypeEarningEventProcessor<TEarningEvent> : EarningEventProcessorBase<TEarningEvent>
        where TEarningEvent : IContractTypeEarningEvent
    {
        protected ApprenticeshipContractTypeEarningEventProcessor(
            IMapper mapper,
            IRequiredPaymentProcessor requiredPaymentProcessor,
            IHoldingBackCompletionPaymentService holdingBackCompletionPaymentService,
            IPaymentHistoryRepository paymentHistoryRepository,
            IApprenticeshipKeyProvider apprenticeshipKeyProvider,
            INegativeEarningService negativeEarningService,
            IPaymentLogger paymentLogger
        ) : base(
            mapper,
            requiredPaymentProcessor,
            holdingBackCompletionPaymentService,
            paymentHistoryRepository,
            apprenticeshipKeyProvider,
            negativeEarningService,
            paymentLogger
        )
        {
        }

        protected override IReadOnlyCollection<(EarningPeriod period, int type)> GetPeriods(TEarningEvent earningEvent)
        {
            var result = new List<(EarningPeriod period, int type)>();

            if (earningEvent.OnProgrammeEarnings != null)
            {
                foreach (var onProgrammeEarning in earningEvent.OnProgrammeEarnings)
                {
                    foreach (var period in onProgrammeEarning.Periods)
                    {
                        result.Add((period, (int) onProgrammeEarning.Type));
                    }
                }
            }

            if (earningEvent.IncentiveEarnings != null)
            {
                foreach (var incentiveEarning in earningEvent.IncentiveEarnings)
                {
                    foreach (var period in incentiveEarning.Periods)
                    {
                        result.Add((period, (int) incentiveEarning.Type));
                    }
                }
            }

            return result;
        }
    }
}