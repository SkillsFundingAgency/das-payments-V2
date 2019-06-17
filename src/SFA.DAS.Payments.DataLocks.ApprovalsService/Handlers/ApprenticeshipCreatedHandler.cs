using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Handlers
{
    public class ApprenticeshipCreatedHandler: IHandleMessages<ApprenticeshipCreatedEvent>
    {
        private readonly IPaymentLogger logger;

        public ApprenticeshipCreatedHandler(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ApprenticeshipCreatedEvent message, IMessageHandlerContext context)
        {
            logger.LogInfo($"Received apprenticeship created: {message.ToJson()} ");
        }
    }
}