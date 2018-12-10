using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing
{
    public interface IPaymentsEventModelBatchService<T> where T : IPaymentsEventModel
    {
        Task RunAsync(CancellationToken cancellationToken);
    }

    public class PaymentsEventModelBatchService<T> : IPaymentsEventModelBatchService<T> where T : IPaymentsEventModel
    {
        private readonly IBatchScopeFactory batchScopeFactory;
        private readonly IPaymentLogger logger;
        private readonly TimeSpan batchInterval;
        private readonly int batchSize;
        private readonly Policy policy;

        public PaymentsEventModelBatchService(IConfigurationHelper configurationHelper, IBatchScopeFactory batchScopeFactory, IPaymentLogger logger)
        {
            this.batchScopeFactory = batchScopeFactory ?? throw new ArgumentNullException(nameof(batchScopeFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var intervalInSeconds = int.Parse(configurationHelper.GetSetting("BatchIntervalInSeconds"));
            batchInterval = TimeSpan.FromSeconds(intervalInSeconds);
            batchSize = int.Parse(configurationHelper.GetSetting("BatchSize"));
            policy = Policy.Handle<Exception>()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(int.Parse(configurationHelper.GetSetting("BatchFailureTimeoutInSeconds"))));
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await policy.ExecuteAsync(() => StorePayments(cancellationToken));
                    await Task.Delay(batchInterval, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogFatal($"Fatal error while storing batch. Error: {ex.Message}", ex);
            }
        }

        private async Task StorePayments(CancellationToken cancellationToken)
        {
            var processedCount = 0;
            do
            {
                using (var scope = batchScopeFactory.Create())
                {
                    try
                    {
                        var processor = scope.GetBatchProcessor<T>(); 
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

            } while (processedCount == batchSize);
        }
    }
}