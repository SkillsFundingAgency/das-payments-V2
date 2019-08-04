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
        Task StorePayments(CancellationToken cancellationToken);
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
                .CircuitBreakerAsync(10, TimeSpan.FromSeconds(int.Parse(configurationHelper.GetSetting("BatchFailureTimeoutInSeconds"))));
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInfo($"PaymentsEventModelBatchService for {typeof(T).Name} started");

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        await policy.ExecuteAsync(() => StorePayments(cancellationToken));
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error while storing batch. Error: {ex.Message}", ex);
                    }

                    await Task.Delay(batchInterval, cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                logger.LogInfo($"Cancellation requested, stopping PaymentsEventModelBatchService for {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                logger.LogFatal($"Fatal error while storing batch. Error: {ex.Message}", ex);
            }
        }

        public async Task StorePayments(CancellationToken cancellationToken)
        {
            int processedCount;
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
                    catch (Exception)
                    {
                        scope.Abort();
                        throw;
                    }
                }
            } while (processedCount == batchSize);
        }
    }
}