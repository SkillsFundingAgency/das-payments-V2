using System.Collections.Generic;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
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
        private readonly long jobId;
        private readonly byte collectionPeriod;
        private readonly short academicYear;

        public PeriodEndSummary(long jobId, byte collectionPeriod, short academicYear)
        {
            this.jobId = jobId;
            this.collectionPeriod = collectionPeriod;
            this.academicYear = academicYear;
        }

        public PeriodEndSummaryModel GetMetrics()
        {
            return new PeriodEndSummaryModel()
            {
                CollectionPeriod = collectionPeriod,
                AcademicYear = academicYear,
                JobId = jobId,
                DcEarnings = new ContractTypeAmounts(),
                HeldBackCompletionPayments = new ContractTypeAmounts(),
                PaymentMetrics = new ContractTypeAmountsVerbose(),
                Payments = new ContractTypeAmounts(),
                YearToDatePayments = new ContractTypeAmounts(),
            };
        }

        public void AddProviderSummaries(List<ProviderPeriodEndSummaryModel> providerSummaries)
        {
        }
    }
}