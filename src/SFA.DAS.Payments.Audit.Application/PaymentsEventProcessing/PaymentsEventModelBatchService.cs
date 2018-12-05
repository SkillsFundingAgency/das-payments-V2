using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing
{
    public interface IPaymentsEventModelBatchService<T> where T : PaymentsEventModel
    {
        Task RunAsync(CancellationToken cancellationToken);
    }

    public class PaymentsEventModelBatchService<T>: IPaymentsEventModelBatchService<T> where T: PaymentsEventModel
    {
        private readonly IBatchScopeFactory batchScopeFactory;
        private readonly IPaymentLogger logger;
        private readonly TimeSpan batchInterval;
        private readonly int batchSize;

        public PaymentsEventModelBatchService(IConfigurationHelper configurationHelper, IBatchScopeFactory batchScopeFactory, IPaymentLogger logger)
        {
            this.batchScopeFactory = batchScopeFactory ?? throw new ArgumentNullException(nameof(batchScopeFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            batchInterval = TimeSpan.FromSeconds(int.Parse(configurationHelper.GetSetting("BatchIntervalInSeconds")));
            batchSize = int.Parse(configurationHelper.GetSetting("BatchSize"));
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int processedCount;
                using (var scope = batchScopeFactory.Create())
                {
                    try
                    {
                        var processor = scope.GetBatchProcessor<T>(); //TODO: move to factory ??
                        processedCount = await processor.Process(batchSize, cancellationToken);
                        await scope.Commit();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error processing batch of payments.  Error: {ex.Message}", ex);
                        scope.Abort();
                        throw;
                    }
                }
                if (processedCount != batchSize)
                    await Task.Delay(batchInterval, cancellationToken);

            }
        }
    }
}