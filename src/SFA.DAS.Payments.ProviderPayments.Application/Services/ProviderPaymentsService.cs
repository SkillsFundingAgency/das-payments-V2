using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class ProviderPaymentsService : IProviderPaymentsService
    {

        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache;
        private readonly IValidateIlrSubmission validateIlrSubmission;
        private readonly IPaymentLogger paymentLogger;
        private readonly ITelemetry telemetry;

        public ProviderPaymentsService(IProviderPaymentsRepository providerPaymentsRepository,
            IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache,
            IValidateIlrSubmission validateIlrSubmission,
            IPaymentLogger paymentLogger,
            ITelemetry telemetry)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.ilrSubmittedEventCache = ilrSubmittedEventCache;
            this.validateIlrSubmission = validateIlrSubmission;
            this.paymentLogger = paymentLogger;
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task ProcessPayment(PaymentModel payment, CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var isCurrentProviderIlr = await IsCurrentProviderIlr(payment.JobId, payment.Ukprn, payment.IlrSubmissionDateTime, cancellationToken);

            if (!isCurrentProviderIlr)
            {
                paymentLogger.LogWarning($"Received out of sequence payment with Job Id {payment.JobId} for Ukprn {payment.Ukprn} ");
                telemetry.TrackEvent("Provider payments service received out of sequence payment");
                return;
            }

            paymentLogger.LogDebug($"Received valid payment with Job Id {payment.JobId} for Ukprn {payment.Ukprn} ");

            await providerPaymentsRepository.SavePayment(payment, cancellationToken);
            stopwatch.Stop();
            telemetry.TrackDuration(GetType().FullName + ".ProcessPayment", stopwatch.Elapsed);
        }

        private async Task<bool> IsCurrentProviderIlr(long jobId, long ukprn, DateTime ilrSubmissionDateTime, CancellationToken cancellationToken)
        {
            var currentIlr = await GetCurrentIlrSubmissionEvent(ukprn, cancellationToken);
            var allowPayment = validateIlrSubmission.IsLatestIlrPayment(new IlrSubmissionValidationRequest
            {
                IncomingPaymentUkprn = ukprn,
                IncomingPaymentSubmissionDate = ilrSubmissionDateTime,
                CurrentIlr = currentIlr
            });
            return allowPayment;
        }

        private async Task<IlrSubmittedEvent> GetCurrentIlrSubmissionEvent(long ukprn, CancellationToken cancellationToken)
        {
            var currentSubmittedIlrConditionalValue = await ilrSubmittedEventCache.TryGet(ukprn.ToString(), cancellationToken);
            return currentSubmittedIlrConditionalValue.HasValue ? currentSubmittedIlrConditionalValue.Value : null;
        }
    }
}
