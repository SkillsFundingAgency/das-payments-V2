using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Metrics.Data;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd
{

    public interface IPeriodEndMetricsRepository
    {
        Task SaveProviderSummaries(List<ProviderPeriodEndSummaryModel> providerSummaries, CancellationToken cancellationToken);
        Task SavePeriodEndSummary(PeriodEndSummaryModel overallPeriodEndSummary, CancellationToken cancellationToken);
        Task<List<ProviderTransactionTypeAmounts>> GetTransactionTypesByContractType(short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
        Task<List<ProviderFundingSourceAmounts>> GetFundingSourceAmountsByContractType(short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
        Task<List<object>> GetDataLockedAmounts
            (short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
    }

    public class PeriodEndMetricsRepository : IPeriodEndMetricsRepository
    {
        private readonly IMetricsPersistenceDataContext persistenceDataContext;
        private readonly IMetricsQueryDataContext queryDataContext;
        private readonly IPaymentLogger logger;


        public PeriodEndMetricsRepository( IMetricsPersistenceDataContext persistenceDataContext, IMetricsQueryDataContext queryDataContext, IPaymentLogger logger)
        {
            this.persistenceDataContext = persistenceDataContext ?? throw new ArgumentNullException(nameof(persistenceDataContext));
            this.queryDataContext = queryDataContext ?? throw new ArgumentNullException(nameof(queryDataContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        Task IPeriodEndMetricsRepository.SavePeriodEndSummary(PeriodEndSummaryModel overallPeriodEndSummary,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProviderTransactionTypeAmounts>> GetTransactionTypesByContractType(short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProviderFundingSourceAmounts>> GetFundingSourceAmountsByContractType(short academicYear, byte collectionPeriod,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<object>> GetDataLockedAmounts(short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPeriodEndMetricsRepository.SaveProviderSummaries(List<ProviderPeriodEndSummaryModel> providerSummaries, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

      
    }
}