using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Repositories
{
    public interface IProviderPaymentsRepository
    {
        Task SavePayment(PaymentModel paymentData, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<PaymentModel>> GetMonthEndPayments(int collectionYear, int collectionPeriodMonth, long ukprn, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<long>> GetMonthEndUkprns(int collectionYear,
            int collectionPeriodMonth, 
                                            CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteOldMonthEndPayment(int collectionYear, 
                                        int collectionPeriodMonth, 
                                        long ukprn,
                                        DateTime currentIlrSubmissionDateTime, 
                                        CancellationToken cancellationToken = default(CancellationToken));
    }
}
