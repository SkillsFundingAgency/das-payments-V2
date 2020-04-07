using System;
using System.Threading.Tasks;
using AutoMapper;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class Act1RedundancyFunctionalSkillEarningsEventHandler: IHandleMessages<Act1RedundancyFunctionalSkillEarningsEvent>
    {
        private readonly IMapper mapper;
        private readonly IPaymentLogger paymentLogger;

        public Act1RedundancyFunctionalSkillEarningsEventHandler(IMapper mapper,IPaymentLogger paymentLogger)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
        }
        public async Task Handle(Act1RedundancyFunctionalSkillEarningsEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Act1RedundancyFunctionalSkillEarningsEvent event. Message Id : {context.MessageId}");
          
            var payableEarning = mapper.Map<PayableFunctionalSkillEarningEvent>(message);
            await context.SendLocal(payableEarning);

            paymentLogger.LogInfo($"Processed Act1RedundancyFunctionalSkillEarningsEvent event. Message Id : {context.MessageId}");
        }
    }
}