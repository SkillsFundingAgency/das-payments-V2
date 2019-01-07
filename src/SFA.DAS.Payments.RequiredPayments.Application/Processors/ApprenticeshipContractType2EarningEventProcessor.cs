using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public class ApprenticeshipContractType2EarningEventProcessor : EarningEventProcessorBase<ApprenticeshipContractType2EarningEvent, RequiredPaymentEvent>
    {
        public ApprenticeshipContractType2EarningEventProcessor(IPaymentKeyService paymentKeyService, IMapper mapper, IPaymentDueProcessor paymentDueProcessor)
            : base(paymentKeyService, mapper, paymentDueProcessor)
        {
        }

        protected override RequiredPaymentEvent CreateRequiredPayment(ApprenticeshipContractType2EarningEvent earningEvent, (EarningPeriod period, int type) periodAndType, Payment[] payments)
        {
            if (Enum.IsDefined(typeof(OnProgrammeEarningType), periodAndType.type))
            {
                var sfaContributionPercentage = periodAndType.period.SfaContributionPercentage.GetValueOrDefault(earningEvent.SfaContributionPercentage);

                // YUCK: refund with 0 earning
                // TODO: work out the better way of doing it
                if (sfaContributionPercentage == 0 && periodAndType.period.Amount == 0 && payments.Length > 0)
                {
                    var sfaContribution = payments.Where(p => p.FundingSource == FundingSourceType.CoInvestedSfa).Sum(p => p.Amount);
                    var employerContribution = payments.Where(p => p.FundingSource == FundingSourceType.CoInvestedEmployer).Sum(p => p.Amount);
                    if (sfaContribution + employerContribution == 0) // protection from div by 0
                    {
                        sfaContributionPercentage = 0;
                    }
                    else
                    {
                        sfaContributionPercentage = sfaContribution / (sfaContribution + employerContribution);
                    }
                }

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
    }
}