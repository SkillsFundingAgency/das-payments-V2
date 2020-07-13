using SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd
{

    public interface IPeriodEndSummaryFactory
    {
        IPeriodEndSummary Create(long jobId, byte collectionPeriod, short academicYear);
    }

    public class PeriodEndSummaryFactory : IPeriodEndSummaryFactory
    {
        public IPeriodEndSummary Create(long jobId, byte collectionPeriod, short academicYear)
        {
            return  new PeriodEndSummary(jobId, collectionPeriod, academicYear);
        }
    }
}