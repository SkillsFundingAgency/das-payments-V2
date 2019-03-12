using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public class FunctionalSkillEarningsEventProcessor : EarningEventProcessorBase<FunctionalSkillEarningsEvent>, IFunctionalSkillEarningsEventProcessor
    {
        public FunctionalSkillEarningsEventProcessor(IPaymentKeyService paymentKeyService, IMapper mapper, IRequiredPaymentService requiredPaymentService)
            : base(paymentKeyService, mapper, requiredPaymentService)
        {
        }

        protected override EarningType GetEarningType(int type)
        {
            return EarningType.Incentive;
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