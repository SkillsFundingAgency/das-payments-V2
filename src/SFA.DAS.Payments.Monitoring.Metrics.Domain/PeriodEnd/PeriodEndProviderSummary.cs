using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd
{
    public interface IPeriodEndProviderSummary
    {
        ProviderPeriodEndSummaryModel GetMetrics();
        void AddDcEarning(IEnumerable<TransactionTypeAmountsBase> providerDcEarningsByContractType);
        void AddTransactionTypes(IEnumerable<TransactionTypeAmountsBase> transactionTypes);
        void AddFundingSourceAmounts(IEnumerable<ProviderFundingSourceAmounts> fundingSourceAmounts);
        void AddDataLockedEarnings(decimal dataLockedEarningsTotal);
    }

    public class PeriodEndProviderSummary : IPeriodEndProviderSummary
    {
        public long Ukprn { get; }
        public long JobId { get; }
        public byte CollectionPeriod { get; }
        public short AcademicYear { get; }
        private List<TransactionTypeAmountsBase> providerDcEarnings;
        private List<TransactionTypeAmountsBase> providerTransactionsTypes;
        private List<ProviderFundingSourceAmounts> providerFundingSourceAmounts;
        private decimal providerDatalockedEarnings;



        public PeriodEndProviderSummary(long ukprn, long jobId, byte collectionPeriod, short academicYear)
        {
            Ukprn = ukprn;
            JobId = jobId;
            CollectionPeriod = collectionPeriod;
            AcademicYear = academicYear;
            providerDcEarnings = new List<TransactionTypeAmountsBase>();
            providerTransactionsTypes = new List<TransactionTypeAmountsBase>();
            providerFundingSourceAmounts = new List<ProviderFundingSourceAmounts>();

        }


        public ProviderPeriodEndSummaryModel GetMetrics()
        {
            return new ProviderPeriodEndSummaryModel();
        }

        public void AddDcEarning(IEnumerable<TransactionTypeAmountsBase> providerDcEarningsByContractType)
        {
            providerDcEarnings = providerDcEarningsByContractType.ToList();
        }

        public void AddTransactionTypes(IEnumerable<TransactionTypeAmountsBase> transactionTypes)
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