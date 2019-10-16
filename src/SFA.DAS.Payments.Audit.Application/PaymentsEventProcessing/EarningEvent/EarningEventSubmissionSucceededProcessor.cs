using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent
{
    public interface IEarningEventSubmissionSucceededProcessor
    {
        Task Process(SubmissionSucceededEvent message, CancellationToken cancellationToken);
    }

    public class EarningEventSubmissionSucceededProcessor: IEarningEventSubmissionSucceededProcessor
    {
        private readonly IPaymentsEventModelBatchService<EarningEventModel> batchService;
        private readonly IPaymentLogger logger;
        private readonly IEarningEventRepository repository;

        public EarningEventSubmissionSucceededProcessor(IPaymentsEventModelBatchService<EarningEventModel> batchService, 
            IPaymentLogger logger, IEarningEventRepository repository)
        {
            this.batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Process(SubmissionSucceededEvent message, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Flushing cached earning events before removing old data for provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
            await batchService.StorePayments(cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Flushed cache. Now removing old earning events for provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
            await repository.RemovePriorEvents(message.Ukprn, message.AcademicYear, message.CollectionPeriod, message.IlrSubmissionDateTime, 
                    cancellationToken)
                .ConfigureAwait(false);
            logger.LogInfo($"Finished removing old earning events for provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
        }
    }
}