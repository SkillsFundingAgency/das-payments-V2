using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using SFA.DAS.Payments.ProviderPayments.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class ProviderPaymentsHandlerService : IProviderPaymentsHandlerService
    {

        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache;
        private readonly IValidatePaymentMessage validatePaymentMessage;
        private readonly IPaymentLogger paymentLogger;
        private readonly IMapper mapper;
        private readonly IProviderPaymentFactory paymentFactory;


        public ProviderPaymentsHandlerService(IProviderPaymentsRepository providerPaymentsRepository,
            IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache,
            IValidatePaymentMessage validatePaymentMessage,
            IPaymentLogger paymentLogger,
            IMapper mapper,
            IProviderPaymentFactory paymentFactory)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.ilrSubmittedEventCache = ilrSubmittedEventCache;
            this.validatePaymentMessage = validatePaymentMessage;
            this.paymentLogger = paymentLogger;
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.paymentFactory = paymentFactory ?? throw new ArgumentNullException(nameof(paymentFactory));
        }

        public async Task ProcessPayment(PaymentModel payment, CancellationToken cancellationToken)
        {
            var currentIlr = await GetCurrentIlrSubmissionEvent(payment.Ukprn, cancellationToken);

            var allowPayment = validatePaymentMessage.IsLatestIlrPayment(new PaymentMessageValidationRequest
            {
                IncomingPaymentJobId = payment.JobId,
                IncomingPaymentUkprn = payment.Ukprn,
                IncomingPaymentSubmissionDate = payment.IlrSubmissionDateTime,
                CurrentIlr = currentIlr
            });

            if (!allowPayment)
            {
                paymentLogger.LogWarning($"Received out of sequence payment with Job Id {payment.JobId} for Ukprn {payment.Ukprn} ");
                return;
            }

            paymentLogger.LogDebug($"Received valid payment with Job Id {payment.JobId} for Ukprn {payment.Ukprn} ");

            await providerPaymentsRepository.SavePayment(payment, cancellationToken);
        }

        public async Task<List<PaymentModel>> GetMonthEndPayments(short collectionYear, byte collectionPeriod, long ukprn, CancellationToken cancellationToken = default(CancellationToken))
        {
            var payments = await providerPaymentsRepository.GetMonthEndPayments(collectionYear, collectionPeriod, ukprn, cancellationToken);
            return payments.ToList();
        }

        private async Task<IlrSubmittedEvent> GetCurrentIlrSubmissionEvent(long ukprn, CancellationToken cancellationToken)
        {
            var currentSubmittedIlrConditionalValue = await ilrSubmittedEventCache.TryGet(ukprn.ToString(), cancellationToken);
            return currentSubmittedIlrConditionalValue.HasValue ? currentSubmittedIlrConditionalValue.Value : null;
        }
    }
}
