using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using SFA.DAS.Payments.ProviderPayments.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class FundingSourceEventHandlerService : IFundingSourceEventHandlerService
    {
        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache;
        private readonly IValidatePaymentMessage validatePaymentMessage;
        private readonly IPaymentLogger paymentLogger;

        public FundingSourceEventHandlerService(IProviderPaymentsRepository providerPaymentsRepository, 
            IDataCache<IlrSubmittedEvent> ilrSubmittedEventCache, 
            IValidatePaymentMessage validatePaymentMessage,
            IPaymentLogger paymentLogger)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.ilrSubmittedEventCache = ilrSubmittedEventCache;
            this.validatePaymentMessage = validatePaymentMessage;
            this.paymentLogger = paymentLogger;
        }

        public async Task ProcessEvent(ProviderPeriodicPayment message, CancellationToken cancellationToken)
        {
            var currentIlr = await GetCurrentIlrSubmissionEvent(message.Ukprn.ToString(), cancellationToken);

            var savePayment = validatePaymentMessage.IsLatestIlrPayment(new PaymentMessageValidationRequest
            {
                IncomingPaymentJobId = message.JobId,
                IncomingPaymentUkprn = message.Ukprn,
                IncomingPaymentSubmissionDate = message.SubmissionDate,
                CurrentIlr = currentIlr
            });

            if (savePayment)
            {
                paymentLogger.LogDebug($"Received valid payment with Job Id {message.JobId} for Ukprn {message.Ukprn} ");

                await providerPaymentsRepository.SavePayment(MapToPaymentEntity(message), cancellationToken);
            }
            else
            {
                paymentLogger.LogWarning($"Received out of sequence payment with Job Id {message.JobId} for Ukprn {message.Ukprn} ");
            }
       
        }

        private PaymentDataEntity MapToPaymentEntity(ProviderPeriodicPayment message)
        {
            var payment = new PaymentDataEntity
            {
                Id = Guid.NewGuid(),
                FundingSource = (int)message.FundingSourceType,
                ContractType = message.ContractType,
                TransactionType = (int)message.OnProgrammeEarningType,
                Amount = message.AmountDue,
                PriceEpisodeIdentifier = message.PriceEpisodeIdentifier,
                CollectionPeriodMonth = message.CollectionPeriod.Month,
                CollectionPeriodName = message.CollectionPeriod.Name,
                CollectionPeriodYear = message.CollectionPeriod.Year,
                DeliveryPeriodMonth = message.DeliveryPeriod.Month,
                DeliveryPeriodYear = message.DeliveryPeriod.Year,
                FrameworkCode = message.LearningAim.FrameworkCode,
                StandardCode = message.LearningAim.StandardCode,
                LearnAimReference = message.LearningAim.Reference,
                LearnerReferenceNumber = message.Learner.ReferenceNumber,
                PathwayCode = message.LearningAim.PathwayCode,
                ProgrammeType = message.LearningAim.ProgrammeType,
                Ukprn = message.Ukprn
            };

            return payment;
        }

        private async Task<IlrSubmittedEvent> GetCurrentIlrSubmissionEvent(string ukprn, CancellationToken cancellationToken)
        {
            var currentSubmittedIlrConditionalValue = await ilrSubmittedEventCache.TryGet(ukprn, cancellationToken);
            return currentSubmittedIlrConditionalValue.HasValue ? currentSubmittedIlrConditionalValue.Value : null;
        }


    }
}
