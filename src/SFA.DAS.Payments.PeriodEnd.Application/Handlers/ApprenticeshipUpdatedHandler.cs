using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.PeriodEnd.Application.Repositories;

namespace SFA.DAS.Payments.PeriodEnd.Application.Handlers
{
    class ApprenticeshipUpdatedHandler : IHandleMessages<ApprenticeshipUpdated>
    {
        private readonly IProvidersRequiringReprocessingRepository repository;
        private readonly IPaymentLogger logger;

        public ApprenticeshipUpdatedHandler(IProvidersRequiringReprocessingRepository repository, IPaymentLogger logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ApprenticeshipUpdated message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Apprenticeship Updated. Ensuring Ukprn: {message.Ukprn} exists on the list for reprocessing");

            await repository.Add(message.Ukprn);

            logger.LogInfo($"Apprenticeship Updated. Finished ensuring that Ukprn: {message.Ukprn} exists on the list for reprocessing");
        }
    }
}
