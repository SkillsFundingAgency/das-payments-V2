using System;
using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public abstract class ApprenticeshipContractTypeEarningEventProcessor<TRequiredPaymentEvent, TEarningEvent> : EarningEventProcessorBase<TEarningEvent>
        where TRequiredPaymentEvent : CalculatedRequiredOnProgrammeAmount, new()
        where TEarningEvent : IContractTypeEarningEvent
    {
        protected ApprenticeshipContractTypeEarningEventProcessor(IPaymentKeyService paymentKeyService, IMapper mapper, IRequiredPaymentService requiredPaymentsService)
            : base(paymentKeyService, mapper, requiredPaymentsService)
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