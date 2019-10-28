using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent
{
    public interface IEarningEventSubmissionFailedProcessor
    {
        Task Process(SubmissionJobFailed message, CancellationToken cancellationToken);
    }

    public class EarningEventSubmissionFailedProcessor : IEarningEventSubmissionFailedProcessor
    {
        private readonly IPaymentsEventModelBatchService<EarningEventModel> batchService;
        private readonly IPaymentLogger logger;
        private readonly IEarningEventRepository repository;

        public EarningEventSubmissionFailedProcessor(IPaymentsEventModelBatchService<EarningEventModel> batchService,
            IPaymentLogger logger, IEarningEventRepository repository)
        {
            this.batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Process(SubmissionJobFailed message, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Flushing cached earning events before removing data for job: {message.JobId}, provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
            await batchService.StorePayments(cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Flushed data. Now removing earning events for job: {message.JobId}, provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
            await repository.RemoveFailedSubmissionEvents(message.JobId, cancellationToken)
                .ConfigureAwait(false);
            logger.LogInfo($"Finished removing earning events for job: {message.JobId}, provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
        }
    }
}