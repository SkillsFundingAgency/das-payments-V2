using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IProviderPaymentsService
    {
        Task ProcessPayment(ProviderPaymentEventModel payment, CancellationToken cancellationToken);
    }

    public class ProviderPaymentsService : IProviderPaymentsService
    {

        private readonly IDataCache<ReceivedProviderEarningsEvent> ilrSubmittedEventCache;
        private readonly IPaymentsEventModelCache<ProviderPaymentEventModel> paymentCache;
        private readonly IValidateIlrSubmission validateIlrSubmission;
        private readonly IPaymentLogger paymentLogger;
        private readonly ITelemetry telemetry;

        public ProviderPaymentsService(
            IDataCache<ReceivedProviderEarningsEvent> ilrSubmittedEventCache,
            IPaymentsEventModelCache<ProviderPaymentEventModel> paymentCache, 
            IValidateIlrSubmission validateIlrSubmission,
            IPaymentLogger paymentLogger,
            ITelemetry telemetry)
        {
            this.ilrSubmittedEventCache = ilrSubmittedEventCache;
            this.paymentCache = paymentCache ?? throw new ArgumentNullException(nameof(paymentCache));
            this.validateIlrSubmission = validateIlrSubmission;
            this.paymentLogger = paymentLogger;
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task ProcessPayment(ProviderPaymentEventModel payment, CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var isCurrentProviderIlr = await IsCurrentProviderIlr(payment.JobId, payment.Ukprn, payment.IlrSubmissionDateTime, cancellationToken).ConfigureAwait(false);

            if (!isCurrentProviderIlr)
            {
                paymentLogger.LogWarning($"Received out of sequence payment with Job Id {payment.JobId} for Ukprn {payment.Ukprn} ");
                telemetry.TrackEvent("Provider payments service received out of sequence payment");
                return;
            }

            paymentLogger.LogVerbose($"Received valid payment with Job Id {payment.JobId} for Ukprn {payment.Ukprn} ");
            await paymentCache.AddPayment(payment, cancellationToken);
            stopwatch.Stop();
            telemetry.TrackDuration(GetType().FullName + ".ProcessPayment", stopwatch.Elapsed);
            paymentLogger.LogInfo($"Finished adding the payment to the cache. EventId: {payment.EventId}, FundingSourceId: {payment.FundingSourceId}, UKPRN: {payment.Ukprn}");
        }

        private async Task<bool> IsCurrentProviderIlr(long jobId, long ukprn, DateTime ilrSubmissionDateTime, CancellationToken cancellationToken)
        {
            var currentIlr = await GetCurrentIlrSubmissionEvent(ukprn, cancellationToken);
            //TODO: this looks like a bit of a hack to me, should be a specific payment validation class
            var allowPayment = validateIlrSubmission.IsLatestIlrPayment(new IlrSubmissionValidationRequest
            {
                IncomingPaymentUkprn = ukprn,
                IncomingPaymentSubmissionDate = ilrSubmissionDateTime,
                CurrentIlr = currentIlr
            });
            return allowPayment;
        }

        private async Task<ReceivedProviderEarningsEvent> GetCurrentIlrSubmissionEvent(long ukprn, CancellationToken cancellationToken)
        {
            var currentSubmittedIlrConditionalValue = await ilrSubmittedEventCache.TryGet(ukprn.ToString(), cancellationToken);
            return currentSubmittedIlrConditionalValue.HasValue ? currentSubmittedIlrConditionalValue.Value : null;
        }
    }
}
