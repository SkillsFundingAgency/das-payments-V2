using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public class FunctionalSkillEarningsEventProcessor : EarningEventProcessorBase<FunctionalSkillEarningsEvent, IncentiveRequiredPaymentEvent>
    {
        public FunctionalSkillEarningsEventProcessor(IPaymentKeyService paymentKeyService, IMapper mapper, IPaymentDueProcessor paymentDueProcessor)
            : base(paymentKeyService, mapper, paymentDueProcessor)
        {
        }

        protected override IncentiveRequiredPaymentEvent CreateRequiredPayment(FunctionalSkillEarningsEvent earningEvent, int type)
        {
            return new IncentiveRequiredPaymentEvent
            {
                Type = (IncentivePaymentType)type,
                ContractType = ContractType.Act2
            };
        }

        protected override IReadOnlyCollection<(EarningPeriod period, int type)> GetPeriods(FunctionalSkillEarningsEvent earningEvent)
        {
            var result = new List<(EarningPeriod period, int type)>();

            foreach (var onProgrammeEarning in earningEvent.Earnings)
            {
                foreach (var period in onProgrammeEarning.Periods)
                {
                    result.Add((period, (int)onProgrammeEarning.Type));
                }
            }

            return result;
        }
    }
}