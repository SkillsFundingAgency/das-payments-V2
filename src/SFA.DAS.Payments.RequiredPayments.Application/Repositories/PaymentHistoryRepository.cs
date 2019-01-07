using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Repositories
{
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public PaymentHistoryRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task<List<PaymentHistoryEntity>> GetPaymentHistory(ApprenticeshipKey apprenticeshipKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            var payments = await dataContext.Payment
                .Where(payment => apprenticeshipKey.Ukprn == payment.Ukprn &&
                            apprenticeshipKey.FrameworkCode == payment.LearningAimFrameworkCode &&
                            apprenticeshipKey.LearnAimRef == payment.LearningAimReference &&
                            apprenticeshipKey.LearnerReferenceNumber == payment.LearnerReferenceNumber &&
                            apprenticeshipKey.PathwayCode == payment.LearningAimPathwayCode &&
                            (int)apprenticeshipKey.ProgrammeType == payment.LearningAimProgrammeType &&
                            apprenticeshipKey.StandardCode == payment.LearningAimStandardCode)
                .Select(payment => new
                {
                    payment.ExternalId,
                    payment.Ukprn,
                    payment.LearnerReferenceNumber,
                    payment.LearningAimReference,
                    payment.TransactionType,
                    payment.PriceEpisodeIdentifier,
                    payment.DeliveryPeriod,
                    payment.CollectionPeriod,
                    payment.Amount,
                    payment.FundingSource
                })
                .ToListAsync(cancellationToken);

            return payments.Select(payment => new PaymentHistoryEntity
            {
                ExternalId = payment.ExternalId,
                Ukprn = payment.Ukprn,
                LearnerReferenceNumber = payment.LearnerReferenceNumber,
                LearnAimReference = payment.LearningAimReference,
                TransactionType = (int)payment.TransactionType,
                PriceEpisodeIdentifier = payment.PriceEpisodeIdentifier,
                DeliveryPeriod = payment.DeliveryPeriod.Name,
                CollectionPeriod = payment.CollectionPeriod.Name,
                Amount = payment.Amount,
                FundingSource = payment.FundingSource
            })
            .ToList();
        }
    }
}