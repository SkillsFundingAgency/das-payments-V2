using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.ProviderPayments.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class ProviderPaymentsHandlerService : IProviderPaymentsHandlerService
    {

        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache;
        private readonly IValidatePaymentMessage validatePaymentMessage;
        private readonly IPaymentLogger paymentLogger;

        public ProviderPaymentsHandlerService(IProviderPaymentsRepository providerPaymentsRepository,
            IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache,
            IValidatePaymentMessage validatePaymentMessage,
            IPaymentLogger paymentLogger)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.ilrSubmittedEventCache = ilrSubmittedEventCache;
            this.validatePaymentMessage = validatePaymentMessage;
            this.paymentLogger = paymentLogger;
        }

        public async Task ProcessPayment(ProviderPeriodicPayment payment, CancellationToken cancellationToken)
        {
            var currentIlr = await GetCurrentIlrSubmissionEvent(payment.Ukprn.ToString(), cancellationToken);

            var savePayment = validatePaymentMessage.IsLatestIlrPayment(new PaymentMessageValidationRequest
            {
                IncomingPaymentJobId = payment.JobId,
                IncomingPaymentUkprn = payment.Ukprn,
                IncomingPaymentSubmissionDate = payment.IlrSubmissionDateTime,
                CurrentIlr = currentIlr
            });

            if (savePayment)
            {
                paymentLogger.LogDebug($"Received valid payment with Job Id {payment.JobId} for Ukprn {payment.Ukprn} ");

                await providerPaymentsRepository.SavePayment(MapToPaymentEntity(payment), cancellationToken);
            }
            else
            {
                paymentLogger.LogWarning($"Received out of sequence payment with Job Id {payment.JobId} for Ukprn {payment.Ukprn} ");
            }

        }

        public async Task<List<ProviderPaymentEvent>> GetMonthEndPayments(short collectionYear, byte collectionPeriod, long ukprn, CancellationToken cancellationToken = default(CancellationToken))
        {
            var payments = await providerPaymentsRepository.GetMonthEndPayments(collectionYear, collectionPeriod, ukprn, cancellationToken);
            return payments.Select(MapToPeriodicPayment).ToList();
        }

        private PaymentModel MapToPaymentEntity(ProviderPeriodicPayment message)
        {
            var payment = new PaymentModel
            {
                ExternalId = Guid.NewGuid(),
                FundingSource = message.FundingSourceType,
                ContractType = (ContractType)message.ContractType,
                TransactionType = (TransactionType)message.OnProgrammeEarningType,
                Amount = message.AmountDue,
                PriceEpisodeIdentifier = message.PriceEpisodeIdentifier,
                CollectionPeriod = message.CollectionPeriod,
                DeliveryPeriod = message.DeliveryPeriod,
                LearningAimFrameworkCode = message.LearningAim.FrameworkCode,
                LearningAimStandardCode = message.LearningAim.StandardCode,
                LearningAimReference = message.LearningAim.Reference,
                LearnerReferenceNumber = message.Learner.ReferenceNumber,
                LearningAimPathwayCode = message.LearningAim.PathwayCode,
                LearningAimProgrammeType = message.LearningAim.ProgrammeType,
                Ukprn = message.Ukprn,
                SfaContributionPercentage = message.SfaContributionPercentage,
                JobId = message.JobId,
                IlrSubmissionDateTime = message.IlrSubmissionDateTime,
                LearnerUln = message.Learner.Uln,
                LearningAimAgreedPrice = message.LearningAim.AgreedPrice,
                LearningAimFundingLineType = message.LearningAim.FundingLineType,
                Earnings = new EarningsModel
                {

                }

            };

            return payment;
        }

        private ProviderPaymentEvent MapToPeriodicPayment(PaymentModel payment)
        {
            return new ProviderPaymentEvent
            {
                Ukprn = payment.Ukprn,
                FundingSourceType = (FundingSourceType)payment.FundingSource,
                AmountDue = payment.Amount,
                CollectionPeriod = new CalendarPeriod((short)payment.CollectionPeriodYear, (byte)payment.CollectionPeriodMonth),
                DeliveryPeriod = new CalendarPeriod((short)payment.DeliveryPeriodYear, (byte)payment.DeliveryPeriodMonth),
                ContractType = (byte)payment.ContractType,
                SfaContributionPercentage = payment.SfaContributionPercentage,
                JobId = payment.JobId,
                LearningAim = new LearningAim
                {
                    ProgrammeType = payment.LearningAimProgrammeType,
                    FrameworkCode = payment.LearningAimFrameworkCode,
                    PathwayCode = payment.LearningAimPathwayCode,
                    StandardCode = payment.LearningAimStandardCode,
                    AgreedPrice = payment.LearningAimAgreedPrice,
                    FundingLineType = payment.LearningAimFundingLineType,
                    Reference = payment.LearningAimReference
                },
                IlrSubmissionDateTime = payment.IlrSubmissionDateTime,
                OnProgrammeEarningType = (OnProgrammeEarningType)payment.TransactionType,
                Learner = new Learner
                {
                    Ukprn = payment.Ukprn,
                    ReferenceNumber = payment.LearnerReferenceNumber,
                    Uln = payment.LearnerUln
                },
                PriceEpisodeIdentifier = payment.PriceEpisodeIdentifier
            };

        }
        private async Task<IlrSubmittedEvent> GetCurrentIlrSubmissionEvent(string ukprn, CancellationToken cancellationToken)
        {
            var currentSubmittedIlrConditionalValue = await ilrSubmittedEventCache.TryGet(ukprn, cancellationToken);
            return currentSubmittedIlrConditionalValue.HasValue ? currentSubmittedIlrConditionalValue.Value : null;
        }
    }
}
