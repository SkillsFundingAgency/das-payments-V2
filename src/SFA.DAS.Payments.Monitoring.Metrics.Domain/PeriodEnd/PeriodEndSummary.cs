using System.Collections.Generic;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd
{
    public interface IPeriodEndSummary
    {
       PeriodEndSummaryModel GetMetrics();
       void AddProviderSummaries(List<ProviderPeriodEndSummaryModel> providerSummaries);
    }

    public class PeriodEndSummary :IPeriodEndSummary
    {
        public PeriodEndSummary(long jobId, byte collectionPeriod, short academicYear)
        {
            
        }

        public PeriodEndSummaryModel GetMetrics()
        {
            return  new PeriodEndSummaryModel();
        }

        public void AddProviderSummaries(List<ProviderPeriodEndSummaryModel> providerSummaries)
        {
            throw new System.NotImplementedException();
        }
    }
}