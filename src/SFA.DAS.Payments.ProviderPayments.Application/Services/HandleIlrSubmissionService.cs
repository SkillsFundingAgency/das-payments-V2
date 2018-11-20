﻿using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class HandleIlrSubmissionService : IHandleIlrSubmissionService
    {

        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache;
        private readonly IValidateIlrSubmission validateIlrSubmission;
        private readonly IPaymentLogger paymentLogger;

        public HandleIlrSubmissionService(IProviderPaymentsRepository providerPaymentsRepository,
            IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache,
            IValidateIlrSubmission validateIlrSubmission,
            IPaymentLogger paymentLogger)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.ilrSubmittedEventCache = ilrSubmittedEventCache;
            this.validateIlrSubmission = validateIlrSubmission;
            this.paymentLogger = paymentLogger;
        }

        public async Task Handle(IlrSubmittedEvent message, CancellationToken cancellationToken)
        {
            try
            {


                var currentIlr = await GetCurrentIlrSubmissionEvent(message.Ukprn, cancellationToken);
                var isNewIlrSubmission = validateIlrSubmission.IsNewIlrSubmission(new IlrSubmissionValidationRequest
                {
                    IncomingPaymentUkprn = message.Ukprn,
                    IncomingPaymentSubmissionDate = message.IlrSubmissionDateTime,
                    CurrentIlr = currentIlr
                });

                if (!isNewIlrSubmission)
                {
                    paymentLogger.LogInfo($"Ignored same Ilr Submission Data for Ukprn {message.Ukprn} and Job Id {message.JobId} Submission already processed");
                    return;
                }

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
            catch (Exception e)
            {
                paymentLogger.LogError($"Error {e} ", e );
            }
        }

        private async Task<IlrSubmittedEvent> GetCurrentIlrSubmissionEvent(long ukprn, CancellationToken cancellationToken)
        {
            var currentSubmittedIlrConditionalValue = await ilrSubmittedEventCache.TryGet(ukprn.ToString(), cancellationToken);
            return currentSubmittedIlrConditionalValue.HasValue ? currentSubmittedIlrConditionalValue.Value : null;
        }
    }
}