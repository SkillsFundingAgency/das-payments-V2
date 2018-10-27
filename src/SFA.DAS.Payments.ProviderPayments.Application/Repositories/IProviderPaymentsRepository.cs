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
        Task<List<PaymentModel>> GetMonthEndPayments(short collectionYear, byte collectionPeriodMonth, long ukprn, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<long>> GetMonthEndUkprns(short collectionYear, 
                                            byte collectionPeriodMonth, 
                                            CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteOldMonthEndPayment(short collectionYear, 
                                        byte collectionPeriodMonth, long ukprn,
                                        DateTime currentIlrSubmissionDateTime, 
                                        CancellationToken cancellationToken = default(CancellationToken));
    }
}
