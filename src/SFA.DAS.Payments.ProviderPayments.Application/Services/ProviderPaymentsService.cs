using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class ProviderPaymentsService : IProviderPaymentsService
    {

        private readonly IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache;
        private readonly IPaymentsEventModelCache<ProviderPaymentEventModel> paymentCache;
        private readonly IValidateIlrSubmission validateIlrSubmission;
        private readonly IPaymentLogger paymentLogger;

        public ProviderPaymentsService(IProviderPaymentsRepository providerPaymentsRepository, IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache, 
            IPaymentsEventModelCache<ProviderPaymentEventModel> paymentCache, IValidateIlrSubmission validateIlrSubmission,
            IPaymentLogger paymentLogger)
        {
            this.ilrSubmittedEventCache = ilrSubmittedEventCache;
            this.paymentCache = paymentCache ?? throw new ArgumentNullException(nameof(paymentCache));
            this.validateIlrSubmission = validateIlrSubmission;
            this.paymentLogger = paymentLogger;
        }

        public async Task ProcessPayment(ProviderPaymentEventModel payment, CancellationToken cancellationToken)
        {
            var isCurrentProviderIlr = await IsCurrentProviderIlr(payment.JobId, payment.Ukprn, payment.IlrSubmissionDateTime, cancellationToken);

            if (!isCurrentProviderIlr)
            {
                paymentLogger.LogWarning($"Received out of sequence payment with Job Id {payment.JobId} for Ukprn {payment.Ukprn} ");
                return;
            }

            paymentLogger.LogVerbose($"Received valid payment with Job Id {payment.JobId} for Ukprn {payment.Ukprn} ");
            await paymentCache.AddPayment(payment);
            paymentLogger.LogInfo("Finished adding the payment to the cache.");
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

        private async Task<IlrSubmittedEvent> GetCurrentIlrSubmissionEvent(long ukprn, CancellationToken cancellationToken)
        {
            var currentSubmittedIlrConditionalValue = await ilrSubmittedEventCache.TryGet(ukprn.ToString(), cancellationToken);
            return currentSubmittedIlrConditionalValue.HasValue ? currentSubmittedIlrConditionalValue.Value : null;
        }
    }
}
