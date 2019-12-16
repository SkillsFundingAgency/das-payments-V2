using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data.RequiredPayment;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.RequiredPayment
{
    public interface IRequiredPaymentEventSubmissionFailedProcessor
    {
        Task Process(SubmissionJobFailed message, CancellationToken cancellationToken);
    }

    public class RequiredPaymentEventSubmissionFailedProcessor : IRequiredPaymentEventSubmissionFailedProcessor
    {
        private readonly IPaymentsEventModelBatchService<RequiredPaymentEventModel> batchService;
        private readonly IPaymentLogger logger;
        private readonly IRequiredPaymentEventRepository repository;

        public RequiredPaymentEventSubmissionFailedProcessor(IPaymentsEventModelBatchService<RequiredPaymentEventModel> batchService,
            IPaymentLogger logger, IRequiredPaymentEventRepository repository)
        {
            this.batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Process(SubmissionJobFailed message, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Flushing cached payments before removing payments for provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
            await batchService.StorePayments(cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Flushed payments. Now removing earning events for job: {message.JobId}, provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
            await repository.RemoveFailedSubmissionEvents(message.JobId, cancellationToken)
                .ConfigureAwait(false);
            logger.LogInfo($"Finished removing earning events for job: {message.JobId}, provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
        }
    }
}