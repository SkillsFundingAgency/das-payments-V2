using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd
{
    public interface IPeriodEndProviderSummary
    {
        ProviderPeriodEndSummaryModel GetMetrics();
        void AddDcEarning(IEnumerable<TransactionTypeAmountsByContractType> providerDcEarningsByContractType);
        void AddTransactionTypes(IEnumerable<TransactionTypeAmountsByContractType> transactionTypes);
        void AddFundingSourceAmounts(IEnumerable<ProviderFundingSourceAmounts> fundingSourceAmounts);
        void AddDataLockedEarnings(decimal dataLockedEarningsTotal);
    }

    public class PeriodEndProviderSummary : IPeriodEndProviderSummary
    {
        public long Ukprn { get; }
        public long JobId { get; }
        public byte CollectionPeriod { get; }
        public short AcademicYear { get; }
        private List<TransactionTypeAmountsByContractType> providerDcEarnings;
        private List<TransactionTypeAmountsByContractType> providerTransactionsTypes;
        private List<ProviderFundingSourceAmounts> providerFundingSourceAmounts;
        private decimal providerDatalockedEarnings;



        public PeriodEndProviderSummary(long ukprn, long jobId, byte collectionPeriod, short academicYear)
        {
            Ukprn = ukprn;
            JobId = jobId;
            CollectionPeriod = collectionPeriod;
            AcademicYear = academicYear;
            providerDcEarnings = new List<TransactionTypeAmountsByContractType>();
            providerTransactionsTypes = new List<TransactionTypeAmountsByContractType>();
            providerFundingSourceAmounts = new List<ProviderFundingSourceAmounts>();

        }


        public ProviderPeriodEndSummaryModel GetMetrics()
        {
            return new ProviderPeriodEndSummaryModel();
        }

        public void AddDcEarning(IEnumerable<TransactionTypeAmountsByContractType> providerDcEarningsByContractType)
        {
            providerDcEarnings = providerDcEarningsByContractType.ToList();
        }

        public void AddTransactionTypes(IEnumerable<TransactionTypeAmountsByContractType> transactionTypes)
        {
            providerTransactionsTypes = transactionTypes.ToList();
        }

        public void AddFundingSourceAmounts(IEnumerable<ProviderFundingSourceAmounts> fundingSourceAmounts)
        {
            providerFundingSourceAmounts = fundingSourceAmounts.ToList();
        }

        public void AddDataLockedEarnings(decimal dataLockedEarningsTotal)
        {
            providerDatalockedEarnings = dataLockedEarningsTotal;
        }
    }
}