using System;
using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public abstract class ApprenticeshipContractTypeEarningEventProcessor<TRequiredPaymentEvent, TEarningEvent> : EarningEventProcessorBase<TEarningEvent>
        where TRequiredPaymentEvent : ApprenticeshipContractTypeRequiredPaymentEvent, new()
        where TEarningEvent : IContractTypeEarningEvent
    {
        protected ApprenticeshipContractTypeEarningEventProcessor(IPaymentKeyService paymentKeyService, IMapper mapper, IPaymentDueProcessor paymentDueProcessor)
            : base(paymentKeyService, mapper, paymentDueProcessor)
        {
        }

        protected override RequiredPaymentEvent CreateRequiredPayment(TEarningEvent earningEvent, (EarningPeriod period, int type) periodAndType, Payment[] payments)
        {
            if (Enum.IsDefined(typeof(OnProgrammeEarningType), periodAndType.type))
            {
                // TODO: work out the better way of doing it
                var sfaContributionPercentage = periodAndType.period.SfaContributionPercentage.GetValueOrDefault(earningEvent.SfaContributionPercentage);
                sfaContributionPercentage = paymentDueProcessor.CalculateSfaContributionPercentage(sfaContributionPercentage, periodAndType.period.Amount, payments);

                var requiredPayment = new TRequiredPaymentEvent
                {
                    OnProgrammeEarningType = (OnProgrammeEarningType) periodAndType.type, 
                    SfaContributionPercentage = sfaContributionPercentage
                };
                return requiredPayment;
            }

            return new IncentiveRequiredPaymentEvent
            {
                Type = (IncentivePaymentType)periodAndType.type,
                ContractType = ContractType.Act2
            };
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