using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Metrics.Data;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd
{

    public interface IPeriodEndMetricsRepository
    {
        void SaveProviderSummaries(List<ProviderPeriodEndSummaryModel> providerSummaries);
        void SavePeriodEndSummary(PeriodEndSummaryModel overallPeriodEndSummary);
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

        public void SaveProviderSummaries(List<ProviderPeriodEndSummaryModel> providerSummaries)
        {
            throw new System.NotImplementedException();
        }

        public void SavePeriodEndSummary(PeriodEndSummaryModel overallPeriodEndSummary)
        {
            throw new System.NotImplementedException();
        }
    }
}