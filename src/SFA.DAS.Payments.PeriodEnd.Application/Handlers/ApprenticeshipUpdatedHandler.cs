using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.PeriodEnd.Application.Services;

namespace SFA.DAS.Payments.PeriodEnd.Application.Handlers
{
    class ApprenticeshipUpdatedHandler : IHandleMessages<ApprenticeshipUpdated>
    {
        private readonly IProviderRequiringReprocessingService service;
        private readonly IPaymentLogger logger;

        public ApprenticeshipUpdatedHandler(IProviderRequiringReprocessingService service, IPaymentLogger logger)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ApprenticeshipUpdated message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Apprenticeship Updated. Ensuring Ukprn: {message.Ukprn} exists on the list for reprocessing");
            await service.AddUkprnIfNotExists(message.Ukprn);
            logger.LogInfo($"Apprenticeship Updated. Finished ensuring that Ukprn: {message.Ukprn} exists on the list for reprocessing");
        }
    }
}
