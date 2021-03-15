using System;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.PeriodEnd.Application.Repositories;

namespace SFA.DAS.Payments.PeriodEnd.Application.Services
{
    public interface IProviderRequiringReprocessingService
    {
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
    }
}
