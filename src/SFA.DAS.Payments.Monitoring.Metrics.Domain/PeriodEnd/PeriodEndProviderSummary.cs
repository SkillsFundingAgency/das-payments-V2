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
        void AddDataLockedAlreadyPaid(decimal dataLockedAlreadyPaidTotal);
        void AddPaymentsYearToDate(ProviderContractTypeAmounts paymentsYearToDate);
        void AddHeldBackCompletionPayments(IEnumerable<ProviderContractTypeAmounts> heldBackCompletionPayments);
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
        private decimal providerDataLockedEarnings;
        private decimal ProviderDataLockedAlreadyPaidTotal;
        private ProviderContractTypeAmounts providerPaymentsYearToDate;
        private List<ProviderContractTypeAmounts> providerHeldBackCompletionPayments;


        public PeriodEndProviderSummary(long ukprn, long jobId, byte collectionPeriod, short academicYear)
        {
            Ukprn = ukprn;
            JobId = jobId;
            CollectionPeriod = collectionPeriod;
            AcademicYear = academicYear;
            providerDcEarnings = new List<TransactionTypeAmountsByContractType>();
            providerTransactionsTypes = new List<TransactionTypeAmountsByContractType>();
            providerFundingSourceAmounts = new List<ProviderFundingSourceAmounts>();
            providerPaymentsYearToDate = new ProviderContractTypeAmounts();
            providerHeldBackCompletionPayments = new List<ProviderContractTypeAmounts>();
        }


        public ProviderPeriodEndSummaryModel GetMetrics()
        {
            return new ProviderPeriodEndSummaryModel
            {
                Ukprn =  Ukprn,
                AcademicYear = AcademicYear,
                CollectionPeriod = CollectionPeriod,
                JobId = JobId,
                DcEarnings = new ContractTypeAmounts(),
                HeldBackCompletionPayments = new ContractTypeAmounts(),
                PaymentMetrics = new ContractTypeAmountsVerbose(),
                Payments = new ContractTypeAmounts(),
                YearToDatePayments = providerPaymentsYearToDate,
                FundingSourceAmounts = new List<ProviderPaymentFundingSourceModel>(),
                TransactionTypeAmounts = new List<ProviderPaymentTransactionModel>(),
                AlreadyPaidDataLockedEarnings = ProviderDataLockedAlreadyPaidTotal,
                DataLockedEarnings = providerDataLockedEarnings,
                TotalDataLockedEarnings = providerDataLockedEarnings - ProviderDataLockedAlreadyPaidTotal
            };
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
            providerDataLockedEarnings = dataLockedEarningsTotal;
        }

        public void AddDataLockedAlreadyPaid(decimal dataLockedAlreadyPaidTotal)
        {
            ProviderDataLockedAlreadyPaidTotal = dataLockedAlreadyPaidTotal;
        }

        public void AddPaymentsYearToDate(ProviderContractTypeAmounts paymentsYearToDate)
        {
            providerPaymentsYearToDate = paymentsYearToDate;
        }

        public void AddHeldBackCompletionPayments(IEnumerable<ProviderContractTypeAmounts> heldBackCompletionPayments)
        {
            providerHeldBackCompletionPayments = heldBackCompletionPayments.ToList();
        }
    }
}