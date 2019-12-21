using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.Metrics.SubmissionService.Handlers
{
    public class SubmissionJobSucceededHandler: IHandleMessages<SubmissionJobSucceeded>
    {
        private readonly IPaymentLogger logger;

        public SubmissionJobSucceededHandler(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(SubmissionJobSucceeded message, IMessageHandlerContext context)
        {
            logger.LogInfo($"Finished handling the SubmissionJobSucceeded event: {message.ToJson()}");
        }
    }
}