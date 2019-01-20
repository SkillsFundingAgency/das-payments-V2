using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core;

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
    }
}
