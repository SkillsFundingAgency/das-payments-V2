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
        void AddTransactionTypes(IEnumerable<ProviderTransactionTypeAmounts> transactionTypes);
        void AddFundingSourceAmounts(IEnumerable<ProviderFundingSourceAmounts> fundingSourceAmounts);
    }

    public class PeriodEndProviderSummary : IPeriodEndProviderSummary
    {
        public long Ukprn { get; }
        public long JobId { get; }
        public byte CollectionPeriod { get; }
        public short AcademicYear { get; }
        private List<ProviderTransactionTypeAmounts> providerDcEarnings;
        private List<ProviderTransactionTypeAmounts> providerTransactionsTypes;
        private List<ProviderFundingSourceAmounts> providerFundingSourceAmounts;



        public PeriodEndProviderSummary(long ukprn, long jobId, byte collectionPeriod, short academicYear)
        {
            Ukprn = ukprn;
            JobId = jobId;
            CollectionPeriod = collectionPeriod;
            AcademicYear = academicYear;
            providerDcEarnings = new List<ProviderTransactionTypeAmounts>();
            providerTransactionsTypes = new List<ProviderTransactionTypeAmounts>();
            providerFundingSourceAmounts = new List<ProviderFundingSourceAmounts>();

        }


        public ProviderPeriodEndSummaryModel GetMetrics()
        {
            return new ProviderPeriodEndSummaryModel();
        }

        public void AddDcEarning(IEnumerable<ProviderTransactionTypeAmounts> providerDcEarningsByContractType)
        {
            providerDcEarnings = providerDcEarningsByContractType.ToList();
        }

        public void AddTransactionTypes(IEnumerable<ProviderTransactionTypeAmounts> transactionTypes)
        {
            providerTransactionsTypes = transactionTypes.ToList();
        }

        public void AddFundingSourceAmounts(IEnumerable<ProviderFundingSourceAmounts> fundingSourceAmounts)
        {
            providerFundingSourceAmounts = fundingSourceAmounts.ToList();
        }
    }
}