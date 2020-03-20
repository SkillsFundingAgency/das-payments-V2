using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.ProviderPayments.Application.Data;

namespace SFA.DAS.Payments.ProviderPayments.Application.Repositories
{
    public interface IProviderPaymentsRepository
    {
        Task SavePayment(PaymentModel paymentData, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<PaymentModel>> GetMonthEndPayments(CollectionPeriod collectionPeriod, long ukprn,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<List<long>> GetMonthEndProviders(CollectionPeriod collectionPeriod,
            CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteOldMonthEndPayment(CollectionPeriod collectionPeriod,
            long ukprn,
            DateTime currentIlrSubmissionDateTime,
            CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteCurrentMonthEndPayment(CollectionPeriod collectionPeriod,
            long ukprn,
            DateTime currentIlrSubmissionDateTime,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves a paged list of payments for a given <paramref name="collectionPeriod"/>
        /// </summary>
        /// <param name="collectionPeriod">The collection period</param>
        /// <param name="pageSize">The number of records to retrieve</param>
        /// <param name="page">0 based page to retrieve</param>
        List<PaymentModelWithRequiredPaymentId> GetMonthEndPayments(CollectionPeriod collectionPeriod, int pageSize,
            int page);

        Task<List<PaymentModel>> GetMonthEndAct1CompletionPayments(CollectionPeriod collectionPeriod, CancellationToken cancellationToken = default(CancellationToken));
    }
}
