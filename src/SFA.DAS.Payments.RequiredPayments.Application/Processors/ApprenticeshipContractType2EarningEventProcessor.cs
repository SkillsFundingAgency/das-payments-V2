using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public class ApprenticeshipContractType2EarningEventProcessor : EarningEventProcessorBase<ApprenticeshipContractType2EarningEvent, RequiredPaymentEvent>, IApprenticeshipContractTypeEarningsEventProcessor
    {
        public ApprenticeshipContractType2EarningEventProcessor(IPaymentKeyService paymentKeyService, IMapper mapper, IPaymentDueProcessor paymentDueProcessor)
            : base(paymentKeyService, mapper, paymentDueProcessor)
        {
        }

        protected override RequiredPaymentEvent CreateRequiredPayment(ApprenticeshipContractType2EarningEvent earningEvent, (EarningPeriod period, int type) periodAndType, Payment[] payments)
        {
            if (Enum.IsDefined(typeof(OnProgrammeEarningType), periodAndType.type))
            {
                // TODO: work out the better way of doing it
                var sfaContributionPercentage = periodAndType.period.SfaContributionPercentage.GetValueOrDefault(earningEvent.SfaContributionPercentage);
                sfaContributionPercentage = paymentDueProcessor.CalculateSfaContributionPercentage(sfaContributionPercentage, periodAndType.period.Amount, payments);

                return new ApprenticeshipContractType2RequiredPaymentEvent
                {
                    OnProgrammeEarningType = (OnProgrammeEarningType) periodAndType.type,
                    SfaContributionPercentage = sfaContributionPercentage,
                };
            }

            return new IncentiveRequiredPaymentEvent
            {
                Type = (IncentivePaymentType)periodAndType.type,
                ContractType = ContractType.Act2
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

        public async Task<ReadOnlyCollection<RequiredPaymentEvent>> ProcessApprenticeshipContractTypeEarningsEventEvent(ApprenticeshipContractTypeEarningsEvent earningEvent, IRepositoryCache<PaymentHistoryEntity[]> repositoryCache, CancellationToken cancellationToken)
        {
            // TODO: move this to automapper profile instead
            return await HandleEarningEvent(earningEvent, repositoryCache, cancellationToken).ConfigureAwait(false);
        }
    }
}