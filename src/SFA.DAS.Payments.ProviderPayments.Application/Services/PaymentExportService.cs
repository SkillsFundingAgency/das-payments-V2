using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Domain.Services;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IPaymentExportService
    {
        Task PerformExportPaymentsAndEarningsToV1(CollectionPeriod collectionPeriod);
        Task PerformMonthEndTrigger(CollectionPeriod collectionPeriod);
    }

    class PaymentExportService : IPaymentExportService
    {
        private readonly ILegacyPaymentsRepository legacyPaymentsRepository;
        private readonly IPaymentExportProgressCache paymentExportProgressCache;
        private readonly int exportBatchSize;
        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IPaymentLogger logger;
        private readonly IPaymentMapper paymentMapper;

        public PaymentExportService(
            ILegacyPaymentsRepository legacyPaymentsRepository, 
            IPaymentExportProgressCache paymentExportProgressCache, 
            IProviderPaymentsRepository providerPaymentsRepository, 
            IPaymentLogger logger, 
            IPaymentMapper paymentMapper,
            IConfigurationHelper configurationHelper)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.legacyPaymentsRepository = legacyPaymentsRepository ?? throw new ArgumentNullException(nameof(legacyPaymentsRepository));
            this.paymentExportProgressCache = paymentExportProgressCache ?? throw new ArgumentNullException(nameof(paymentExportProgressCache));
            exportBatchSize = configurationHelper.GetSettingOrDefault("ExportBatchSize", 10000);
            this.logger = logger;
            this.paymentMapper = paymentMapper ?? throw new ArgumentNullException(nameof(paymentMapper));
        }

        public async Task PerformExportPaymentsAndEarningsToV1(CollectionPeriod collectionPeriod)
        {
            logger.LogVerbose($"Started V1 payments export for collection period {collectionPeriod}");

            while (true)
            {
                int page = -1;
                try
                {
                    page = await paymentExportProgressCache.GetPage(collectionPeriod.AcademicYear, collectionPeriod.Period);
                    logger.LogVerbose($"Starting with page: {page}");

                    var payments = providerPaymentsRepository.GetMonthEndPayments(collectionPeriod, exportBatchSize, page);

                    if (payments.Count == 0)
                    {
                        logger.LogVerbose($"Finished exporting payments to V1 for collection period: {collectionPeriod}");
                        break;
                    }

                    logger.LogVerbose($"Found {payments.Count} payments to process");

                    var result = paymentMapper.MapV2Payments(payments);
                    await legacyPaymentsRepository
                        .WritePaymentInformation(result.payments, result.requiredPayments, result.earnings)
                        .ConfigureAwait(false);

                    logger.LogVerbose($"Completed write for page: {page} for collection period: {collectionPeriod}");

                    await paymentExportProgressCache
                        .IncrementPage(collectionPeriod.AcademicYear, collectionPeriod.Period)
                        .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    logger.LogError($"Error processing page: {page} during V1 legacy export", e);
                    throw;
                }
            }

            logger.LogVerbose($"Completed V1 payments export for collection period {collectionPeriod}");
        }

        public async Task PerformMonthEndTrigger(CollectionPeriod collectionPeriod)
        {
            logger.LogVerbose($"Triggering month end flag for {collectionPeriod}");
            await legacyPaymentsRepository.WriteMonthEndTrigger(collectionPeriod);
            logger.LogVerbose($"Completed month end flag set for {collectionPeriod}");
        }
    }
}
