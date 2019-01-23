using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public class FunctionalSkillEarningsEventProcessor : EarningEventProcessorBase<FunctionalSkillEarningsEvent>, IFunctionalSkillEarningsEventProcessor
    {
        public FunctionalSkillEarningsEventProcessor(IPaymentKeyService paymentKeyService, IMapper mapper, IPaymentDueProcessor paymentDueProcessor)
            : base(paymentKeyService, mapper, paymentDueProcessor)
        {
        }

        protected override RequiredPaymentEvent CreateRequiredPayment(FunctionalSkillEarningsEvent earningEvent, (EarningPeriod period, int type) periodAndType, Payment[] payments)
        {
            return new IncentiveRequiredPaymentEvent
            {
                Type = (IncentivePaymentType)periodAndType.type,
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