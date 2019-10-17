using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data.FundingSource;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource
{
    public interface IFundingSourceEventSubmissionFailedProcessor
    {
        Task Process(SubmissionFailedEvent message, CancellationToken cancellationToken);
    }

    public class FundingSourceEventSubmissionFailedProcessor : IFundingSourceEventSubmissionFailedProcessor
    {
        private readonly IPaymentsEventModelBatchService<FundingSourceEventModel> batchService;
        private readonly IPaymentLogger logger;
        private readonly IFundingSourceEventRepository repository;

        public FundingSourceEventSubmissionFailedProcessor(IPaymentsEventModelBatchService<FundingSourceEventModel> batchService,
            IPaymentLogger logger, IFundingSourceEventRepository repository)
        {
            this.batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Process(SubmissionFailedEvent message, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Flushing cache before removing funding source events for provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
            await batchService.StorePayments(cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Flushed cache. Now removing funding source events for job: {message.JobId}, provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
            await repository.RemoveFailedSubmissionEvents(message.JobId, cancellationToken)
                .ConfigureAwait(false);
            logger.LogInfo($"Finished removing funding source events for job: {message.JobId}, provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
        }
    }
}