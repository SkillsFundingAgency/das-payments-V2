using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class FundingSourceEventHandlerService : IFundingSourceEventHandlerService
    {
        private readonly IProviderPaymentsRepository providerPaymentsRepository;


        public FundingSourceEventHandlerService(IProviderPaymentsRepository providerPaymentsRepository)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
        }

        public async Task ProcessEvent(FundingSourcePaymentEvent message, CancellationToken cancellationToken)
        {
            var payment = new PaymentDataEntity
            {
                Id = Guid.NewGuid(),
                FundingSource = (int)message.FundingSourceType,
                ContractType = message.ContractType,
                TransactionType =  (int)message.OnProgrammeEarningType,
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
                Ukprn =  message.Ukprn
            };

            await providerPaymentsRepository.SavePayment(payment, cancellationToken);
        }
    }
}
