using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd
{
    public interface IPeriodEndProviderSummary
    {
        ProviderPeriodEndSummaryModel GetMetrics();
        void AddDcEarnings(IEnumerable<TransactionTypeAmountsByContractType> providerDcEarningsByContractType);
        void AddTransactionTypes(IEnumerable<TransactionTypeAmountsByContractType> transactionTypes);
        void AddFundingSourceAmounts(IEnumerable<ProviderFundingSourceAmounts> fundingSourceAmounts);
        void AddDataLockedEarnings(decimal dataLockedEarningsTotal);
        void AddPeriodEndProviderDataLockTypeCounts(PeriodEndProviderDataLockTypeCounts periodEndProviderDataLockTypeCounts);
        void AddDataLockedAlreadyPaid(decimal dataLockedAlreadyPaidTotal);
        void AddPaymentsYearToDate(ProviderContractTypeAmounts paymentsYearToDate);
        void AddHeldBackCompletionPayments(ProviderContractTypeAmounts heldBackCompletionPayments);
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
        private decimal providerDataLockedAlreadyPaidTotal;
        private ProviderContractTypeAmounts providerPaymentsYearToDate;
        private ProviderContractTypeAmounts providerHeldBackCompletionPayments;
        private PeriodEndProviderDataLockTypeCounts periodEndProviderDataLockTypeCounts;

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
            providerHeldBackCompletionPayments = new ProviderContractTypeAmounts();
            periodEndProviderDataLockTypeCounts = new PeriodEndProviderDataLockTypeCounts();
        }


        public ProviderPeriodEndSummaryModel GetMetrics()
        {
            var result =  new ProviderPeriodEndSummaryModel
            {
                Ukprn = Ukprn,
                AcademicYear = AcademicYear,
                CollectionPeriod = CollectionPeriod,
                JobId = JobId,
                DcEarnings = GetDcEarnings(),
                HeldBackCompletionPayments = providerHeldBackCompletionPayments,
                PaymentMetrics = new ContractTypeAmountsVerbose(),
                Payments = GetPaymentTotals(),
                YearToDatePayments = providerPaymentsYearToDate,
                AlreadyPaidDataLockedEarnings = providerDataLockedAlreadyPaidTotal,
                AdjustedDataLockedEarnings = providerDataLockedEarnings - providerDataLockedAlreadyPaidTotal,
                TotalDataLockedEarnings = providerDataLockedEarnings,
                FundingSourceAmounts = GetFundingSourceAmounts(),
                TransactionTypeAmounts = GetTransactionTypeAmounts(),
                DataLockTypeCounts = periodEndProviderDataLockTypeCounts
            };

            result.PaymentMetrics = Helpers.CreatePaymentMetrics(result);
            result.Percentage = result.PaymentMetrics.Percentage;
            return result;
        }

        private ContractTypeAmounts GetPaymentTotals()
        {
            return new ContractTypeAmounts()
            {
                ContractType1 = providerTransactionsTypes.FirstOrDefault(x=>x.ContractType == ContractType.Act1)?.Total ??0m,
                ContractType2 = providerTransactionsTypes.FirstOrDefault(x=>x.ContractType == ContractType.Act2)?.Total ??0m,
            };
        }

        private List<ProviderPaymentFundingSourceModel> GetFundingSourceAmounts()
        {
            return providerFundingSourceAmounts.Select(amounts => new ProviderPaymentFundingSourceModel
            {
                ContractType = amounts.ContractType,
                FundingSource1 = amounts.FundingSource1,
                FundingSource2 = amounts.FundingSource2,
                FundingSource3 = amounts.FundingSource3,
                FundingSource4 = amounts.FundingSource4,
                FundingSource5 = amounts.FundingSource5
            }).ToList();
        }

        private List<ProviderPaymentTransactionModel> GetTransactionTypeAmounts()
        {
            return providerTransactionsTypes.Select(amounts => new ProviderPaymentTransactionModel
            {
                TransactionTypeAmounts = amounts,
                }).ToList();
        }

        private ContractTypeAmountsVerbose GetDcEarnings()
        {
            var contractTypes = providerDcEarnings.GroupBy(earning => earning.ContractType)
                .Select(g => new {ContractType = g.Key, Amount = g.Sum(x => x.Total)})
                .ToList();
            var result = new ContractTypeAmountsVerbose
            {
                ContractType1 = contractTypes.FirstOrDefault(x => x.ContractType == ContractType.Act1)?.Amount ?? 0,
                ContractType2 = contractTypes.FirstOrDefault(x => x.ContractType == ContractType.Act2)?.Amount ?? 0
            };
            return result;
        }

        public void AddDcEarnings(IEnumerable<TransactionTypeAmountsByContractType> providerDcEarningsByContractType)
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

        public void AddPeriodEndProviderDataLockTypeCounts(PeriodEndProviderDataLockTypeCounts periodEndProviderDataLockTypeCounts)
        {
            this.periodEndProviderDataLockTypeCounts = periodEndProviderDataLockTypeCounts;
        }

        public void AddDataLockedAlreadyPaid(decimal dataLockedAlreadyPaidTotal)
        {
            providerDataLockedAlreadyPaidTotal = dataLockedAlreadyPaidTotal;
        }

        public void AddPaymentsYearToDate(ProviderContractTypeAmounts paymentsYearToDate)
        {
            providerPaymentsYearToDate = paymentsYearToDate;
        }

        public void AddHeldBackCompletionPayments(ProviderContractTypeAmounts heldBackCompletionPayments)
        {
            providerHeldBackCompletionPayments = heldBackCompletionPayments;
        }
    }
}