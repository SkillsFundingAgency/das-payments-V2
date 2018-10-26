using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class ProviderPaymentsHandlerService : IProviderPaymentsHandlerService
    {

        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache;
        private readonly IValidateIlrSubmission validateIlrSubmission;
        private readonly IPaymentLogger paymentLogger;

        public ProviderPaymentsHandlerService(IProviderPaymentsRepository providerPaymentsRepository,
            IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache,
            IValidateIlrSubmission validateIlrSubmission,
            IPaymentLogger paymentLogger)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.ilrSubmittedEventCache = ilrSubmittedEventCache;
            this.validateIlrSubmission = validateIlrSubmission;
            this.paymentLogger = paymentLogger;
        }

        public async Task ProcessPayment(PaymentModel payment, CancellationToken cancellationToken)
        {
            var isCurrentProviderIlr = await IsCurrentProviderIlr(payment.JobId, payment.Ukprn, payment.IlrSubmissionDateTime, cancellationToken);

            if (!isCurrentProviderIlr)
            {
                paymentLogger.LogWarning($"Received out of sequence payment with Job Id {payment.JobId} for Ukprn {payment.Ukprn} ");
                return;
            }

            paymentLogger.LogDebug($"Received valid payment with Job Id {payment.JobId} for Ukprn {payment.Ukprn} ");

            await providerPaymentsRepository.SavePayment(payment, cancellationToken);
        }

        public async Task HandleIlrSubMission(IlrSubmittedEvent message, CancellationToken cancellationToken)
        {
            var isCurrentProviderIlr = await IsCurrentProviderIlr(message.JobId, message.Ukprn, message.IlrSubmissionDateTime, cancellationToken);

            if (!isCurrentProviderIlr)
            {
                paymentLogger.LogInfo($"Updating current Ilr Submission Data for Ukprn {message.Ukprn} and Job Id {message.JobId}");

                await ilrSubmittedEventCache.Clear(message.Ukprn.ToString(), cancellationToken);
                await ilrSubmittedEventCache.Add(message.Ukprn.ToString(), message, cancellationToken);

                paymentLogger.LogDebug($"Successfully Updated current Ilr Submission Data for Ukprn {message.Ukprn} and Job Id {message.JobId}");

                await providerPaymentsRepository.DeleteOldMonthEndPayment(message.CollectionPeriod.Year,
                                                                           message.CollectionPeriod.Month,
                                                                            message.Ukprn,
                                                                            message.IlrSubmissionDateTime,
                                                                            cancellationToken);

                paymentLogger.LogInfo($"Successfully Deleted Old Month End Payment for Ukprn {message.Ukprn} and Job Id {message.JobId}");
            }

        }
        public async Task<List<PaymentModel>> GetMonthEndPayments(short collectionYear, byte collectionPeriod, long ukprn, CancellationToken cancellationToken = default(CancellationToken))
        {
            var payments = await providerPaymentsRepository.GetMonthEndPayments(collectionYear, collectionPeriod, ukprn, cancellationToken);
            return payments.ToList();
        }
        private async Task<bool> IsCurrentProviderIlr(long jobId, long ukprn, DateTime ilrSubmissionDateTime, CancellationToken cancellationToken)
        {
            var currentIlr = await GetCurrentIlrSubmissionEvent(ukprn, cancellationToken);
            var allowPayment = validateIlrSubmission.IsLatestIlrPayment(new IlrSubmissionValidationRequest
            {
                IncomingPaymentJobId = jobId,
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
