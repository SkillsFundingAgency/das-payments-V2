using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class PeriodEndStartedHandler: IHandleMessages<PeriodEndStartedEvent>
    {
        private readonly IPaymentLogger logger;

        public PeriodEndStartedHandler(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(PeriodEndStartedEvent message, IMessageHandlerContext context)
        {
            logger.LogInfo($"Received period end started event. Details: {message.ToJson()}");
            //TODO: when regenerate dlock after apprenticeship updated feature is added use this to disable until period end finished
            return Task.CompletedTask;
        }
    }
}