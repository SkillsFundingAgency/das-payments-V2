using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data.RequiredPayment;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.RequiredPayment
{
    public interface IRequiredPaymentEventSubmissionSucceededProcessor
    {
        Task Process(SubmissionSucceededEvent message, CancellationToken cancellationToken);
    }

    public class RequiredPaymentEventSubmissionSucceededProcessor : IRequiredPaymentEventSubmissionSucceededProcessor
    {
        private readonly IPaymentsEventModelBatchService<RequiredPaymentEventModel> batchService;
        private readonly IPaymentLogger logger;
        private readonly IRequiredPaymentEventRepository repository;

        public RequiredPaymentEventSubmissionSucceededProcessor(IPaymentsEventModelBatchService<RequiredPaymentEventModel> batchService,
            IPaymentLogger logger, IRequiredPaymentEventRepository repository)
        {
            this.batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Process(SubmissionSucceededEvent message, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Flushing cached payments before removing required payments for provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
            await batchService.StorePayments(cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Flushed cache. Now removing old required payments for provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
            await repository.RemovePriorEvents(message.Ukprn, message.AcademicYear, message.CollectionPeriod, message.IlrSubmissionDateTime,
                    cancellationToken)
                .ConfigureAwait(false);
            logger.LogInfo($"Finished removing old required payments events for provider: {message.Ukprn}, collection period: {message.CollectionPeriod}");
        }
    }
}