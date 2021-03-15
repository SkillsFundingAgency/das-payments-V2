using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.PeriodEnd.Application.Repositories;

namespace SFA.DAS.Payments.PeriodEnd.Application.Services
{
    public interface IProviderRequiringReprocessingService
    {
        Task AddUkprnIfNotExists(long ukprn);
    }

    public class ProviderRequiringReprocessingService : IProviderRequiringReprocessingService
    {
        private readonly IProvidersRequiringReprocessingRepository repository;
        private readonly IPaymentLogger logger;

        public ProviderRequiringReprocessingService(IProvidersRequiringReprocessingRepository repository, IPaymentLogger logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddUkprnIfNotExists(long ukprn)
        {
            logger.LogDebug($"Adding Ukprn to ProviderRequiringReprocessing table for Ukprn: {ukprn}");
            var record = await repository.GetExisting(ukprn);
            if (record == null)
            {
                logger.LogDebug($"No record found for provider: {ukprn} - adding record to ProviderRequiringReprocessing");
                await repository.Add(ukprn);
            }
            logger.LogInfo($"Finished adding Ukprn to ProviderRequiringReprocessing table for Ukprn: {ukprn}");
        }
    }
}
