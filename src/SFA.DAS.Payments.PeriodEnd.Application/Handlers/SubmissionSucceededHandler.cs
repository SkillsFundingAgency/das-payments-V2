using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.PeriodEnd.Application.Repositories;

namespace SFA.DAS.Payments.PeriodEnd.Application.Handlers
{
    class SubmissionSucceededHandler : IHandleMessages<SubmissionJobSucceeded>
    {
        private readonly IPeriodEndRepository repository;
        private readonly IPaymentLogger logger;

        public SubmissionSucceededHandler(IPeriodEndRepository repository, IPaymentLogger logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(SubmissionJobSucceeded message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Handling SubmissionJobSucceeded event for Ukprn: {message.Ukprn}");
            await repository.RemoveUkrpnFromReprocessingList(message.Ukprn);
            logger.LogInfo($"Finished handling SubmissionJobSucceeded event for Ukprn: {message.Ukprn}");
        }
    }
}
