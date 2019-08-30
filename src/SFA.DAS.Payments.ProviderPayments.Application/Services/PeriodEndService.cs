using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IPeriodEndService
    {
        Task<List<ProcessProviderMonthEndCommand>> GenerateProviderMonthEndCommands(PeriodEndStoppedEvent message);
    }

    public class PeriodEndService: IPeriodEndService
    {
        private readonly IPaymentLogger logger;
        private readonly IProviderPaymentsRepository repository;

        public PeriodEndService(IPaymentLogger  logger, IProviderPaymentsRepository repository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<ProcessProviderMonthEndCommand>> GenerateProviderMonthEndCommands(PeriodEndStoppedEvent message)
        {
            logger.LogInfo($"now building Provider Period End Commands for collection period: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear:0000}, job: {message.JobId}");
            var providers = await repository.GetMonthEndProviders(message.CollectionPeriod, CancellationToken.None)
                .ConfigureAwait(false);
            var commands = new List<ProcessProviderMonthEndCommand>();
            foreach (var provider in providers)
            {
                logger.LogDebug($"Creating period end command for provider: {provider}");
                commands.Add(new ProcessProviderMonthEndCommand
                {
                    Ukprn = provider,
                    CollectionPeriod = message.CollectionPeriod,
                    JobId = message.JobId
                });
            }
            logger.LogInfo($"Finished creating the provider period end events for collection period: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear:0000}, job: {message.JobId}");
            return commands;
        }
    }
}