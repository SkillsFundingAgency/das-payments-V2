using SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd
{

    public interface IPeriodEndSummaryFactory
    {
        IPeriodEndSummary CreatePeriodEndSummary(long jobId, byte collectionPeriod, short academicYear);
        IPeriodEndProviderSummary CreatePeriodEndProviderSummary(long ukprn,long jobId, byte collectionPeriod, short academicYear);
    }

    public class PeriodEndSummaryFactory : IPeriodEndSummaryFactory
    {
        public IPeriodEndSummary CreatePeriodEndSummary(long jobId, byte collectionPeriod, short academicYear)
        {
            return new PeriodEndSummary(jobId, collectionPeriod, academicYear);
        }

        public IPeriodEndProviderSummary CreatePeriodEndProviderSummary(long ukprn, long jobId, byte collectionPeriod, short academicYear)
        {
            return new PeriodEndProviderSummary(ukprn, jobId, collectionPeriod, academicYear);
        }
    }
}