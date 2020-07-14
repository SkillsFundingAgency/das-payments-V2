using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd
{
    public interface IPeriodEndProviderSummary
    {
        ProviderPeriodEndSummaryModel GetMetrics();
        void AddDcEarning(IEnumerable<ProviderTransactionTypeAmounts> providerDcEarningsByContractType);
    }

    public class PeriodEndProviderSummary : IPeriodEndProviderSummary
    {
        public long Ukprn { get; }
        public long JobId { get; }
        public byte CollectionPeriod { get; }
        public short AcademicYear { get; }
        private List<ProviderTransactionTypeAmounts> DcEarnings;



        public PeriodEndProviderSummary(long ukprn, long jobId, byte collectionPeriod, short academicYear)
        {
            this.Ukprn = ukprn;
            this.JobId = jobId;
            this.CollectionPeriod = collectionPeriod;
            this.AcademicYear = academicYear;
            this.DcEarnings = new List<ProviderTransactionTypeAmounts>();
        }


        public ProviderPeriodEndSummaryModel GetMetrics()
        {
            return new ProviderPeriodEndSummaryModel();
        }

        public void AddDcEarning(IEnumerable<ProviderTransactionTypeAmounts> providerDcEarningsByContractType)
        {
            DcEarnings = providerDcEarningsByContractType.ToList();
        }
    }
}