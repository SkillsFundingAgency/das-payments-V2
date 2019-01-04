using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public class ApprenticeshipContractType2EarningEventProcessor : EarningEventProcessorBase<ApprenticeshipContractType2EarningEvent, ApprenticeshipContractType2RequiredPaymentEvent>
    {
        public ApprenticeshipContractType2EarningEventProcessor(IPaymentKeyService paymentKeyService, IMapper mapper, IPaymentDueProcessor paymentDueProcessor)
            : base(paymentKeyService, mapper, paymentDueProcessor)
        {
        }

        protected override ApprenticeshipContractType2RequiredPaymentEvent CreateRequiredPayment(ApprenticeshipContractType2EarningEvent paymentDue, int type)
        {
            return new ApprenticeshipContractType2RequiredPaymentEvent
            {
                OnProgrammeEarningType = (OnProgrammeEarningType)type,
                SfaContributionPercentage = paymentDue.SfaContributionPercentage,
            };
        }

        protected override IReadOnlyCollection<(EarningPeriod period, int type)> GetPeriods(ApprenticeshipContractType2EarningEvent earningEvent)
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