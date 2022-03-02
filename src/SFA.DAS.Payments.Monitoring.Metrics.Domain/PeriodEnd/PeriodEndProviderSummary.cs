using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd
{
    public interface IPeriodEndProviderSummary
    {
        ProviderPeriodEndSummaryModel GetMetrics();
        void AddDcEarnings(IEnumerable<TransactionTypeAmountsByContractType> providerDcEarningsByContractType);
        void AddTransactionTypes(IEnumerable<TransactionTypeAmountsByContractType> transactionTypes);
        void AddFundingSourceAmounts(IEnumerable<ProviderFundingSourceAmounts> fundingSourceAmounts);
        void AddDataLockedEarnings(ProviderFundingLineTypeAmounts dataLockedEarningsTotal);
        void AddPeriodEndProviderDataLockTypeCounts(PeriodEndProviderDataLockTypeCounts periodEndProviderDataLockTypeCounts);
        void AddLearnerNegativeEarnings(List<ProviderNegativeEarningsLearnerDcEarningAmounts> negativeLearnerEarnings);
        void AddLearnerPayments(List<ProviderNegativeEarningsLearnerContractTypeAmounts> learnerPayments);
        void AddLearnerDataLockedEarnings(List<ProviderNegativeEarningsLearnerDataLockFundingLineTypeAmounts> learnerDataLocks);
        void AddDataLockedAlreadyPaid(ProviderFundingLineTypeAmounts dataLockedAlreadyPaidTotal);
        void AddPaymentsYearToDate(ProviderContractTypeAmounts paymentsYearToDate);
        void AddHeldBackCompletionPayments(ProviderContractTypeAmounts heldBackCompletionPayments);
        void AddInLearningCount(ProviderInLearningTotal inLearningTotal);
    }

    public class PeriodEndProviderSummary : IPeriodEndProviderSummary
    {
        public long Ukprn { get; }
        public long JobId { get; }
        public byte CollectionPeriod { get; }
        public short AcademicYear { get; }
        private List<ProviderNegativeEarningsLearnerDcEarningAmounts> providerLearnerNegativeDcEarnings;
        private List<ProviderNegativeEarningsLearnerContractTypeAmounts> providerLearnerPayments;
        private List<ProviderNegativeEarningsLearnerDataLockFundingLineTypeAmounts> providerLearnerDataLockedEarnings;
        private List<TransactionTypeAmountsByContractType> providerDcEarnings;
        private List<TransactionTypeAmountsByContractType> providerTransactionsTypes;
        private List<ProviderFundingSourceAmounts> providerFundingSourceAmounts;
        private ProviderFundingLineTypeAmounts providerDataLockedEarnings;
        private ProviderFundingLineTypeAmounts providerDataLockedAlreadyPaidTotals;
        private ProviderContractTypeAmounts providerPaymentsYearToDate;
        private ProviderContractTypeAmounts providerHeldBackCompletionPayments;
        private PeriodEndProviderDataLockTypeCounts periodEndProviderDataLockTypeCounts;
        private int? inLearning;
        private readonly NegativeEarningsContractTypeAmounts negativeEarnings;
        private readonly ProviderFundingLineTypeAmounts negativeDataLocks;

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
            negativeEarnings = new NegativeEarningsContractTypeAmounts();
            negativeDataLocks = new ProviderFundingLineTypeAmounts();
        }

        public ProviderPeriodEndSummaryModel GetMetrics()
        {
            var result = new ProviderPeriodEndSummaryModel
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

                AlreadyPaidDataLockedEarnings = providerDataLockedAlreadyPaidTotals.Total,
                AlreadyPaidDataLockedEarnings16To18 = providerDataLockedAlreadyPaidTotals.FundingLineType16To18Amount,
                AlreadyPaidDataLockedEarnings19Plus = providerDataLockedAlreadyPaidTotals.FundingLineType19PlusAmount,

                TotalDataLockedEarnings = providerDataLockedEarnings.Total,
                TotalDataLockedEarnings16To18 = providerDataLockedEarnings.FundingLineType16To18Amount,
                TotalDataLockedEarnings19Plus = providerDataLockedEarnings.FundingLineType19PlusAmount,

                FundingSourceAmounts = GetFundingSourceAmounts(),
                TransactionTypeAmounts = GetTransactionTypeAmounts(),
                DataLockTypeCounts = periodEndProviderDataLockTypeCounts,
                InLearning = inLearning,
                NegativeEarnings = negativeEarnings
            };
            
            ApplyNegativeEarningsAdjustments(result);
            CalculateAdjustedDataLocks(result);

            result.PaymentMetrics = Helpers.CreatePaymentMetrics(result);
            result.Percentage = result.PaymentMetrics.Percentage;
            return result;
        }

        private ContractTypeAmounts GetPaymentTotals()
        {
            return new ContractTypeAmounts()
            {
                ContractType1 = providerTransactionsTypes.FirstOrDefault(x => x.ContractType == ContractType.Act1)?.Total ?? 0m,
                ContractType2 = providerTransactionsTypes.FirstOrDefault(x => x.ContractType == ContractType.Act2)?.Total ?? 0m,
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
                .Select(g => new { ContractType = g.Key, Amount = g.Sum(x => x.Total) })
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

        public void AddDataLockedEarnings(ProviderFundingLineTypeAmounts dataLockedEarningsTotal)
        {
            providerDataLockedEarnings = dataLockedEarningsTotal;
        }

        public void AddPeriodEndProviderDataLockTypeCounts(PeriodEndProviderDataLockTypeCounts providerDataLockTypeCounts)
        {
            this.periodEndProviderDataLockTypeCounts = providerDataLockTypeCounts;
        }

        public void AddLearnerNegativeEarnings(List<ProviderNegativeEarningsLearnerDcEarningAmounts> negativeLearnerEarnings)
        {
            providerLearnerNegativeDcEarnings = negativeLearnerEarnings;
        }

        public void AddLearnerPayments(List<ProviderNegativeEarningsLearnerContractTypeAmounts> learnerPayments)
        {
            providerLearnerPayments = learnerPayments;
        }

        public void AddLearnerDataLockedEarnings(List<ProviderNegativeEarningsLearnerDataLockFundingLineTypeAmounts> learnerDataLocks)
        {
            providerLearnerDataLockedEarnings = learnerDataLocks;
        }

        public void AddDataLockedAlreadyPaid(ProviderFundingLineTypeAmounts dataLockedAlreadyPaidTotal)
        {
            providerDataLockedAlreadyPaidTotals = dataLockedAlreadyPaidTotal;
        }

        public void AddPaymentsYearToDate(ProviderContractTypeAmounts paymentsYearToDate)
        {
            providerPaymentsYearToDate = paymentsYearToDate;
        }

        public void AddHeldBackCompletionPayments(ProviderContractTypeAmounts heldBackCompletionPayments)
        {
            providerHeldBackCompletionPayments = heldBackCompletionPayments;
        }

        public void AddInLearningCount(ProviderInLearningTotal inLearningTotal)
        {
            inLearning = inLearningTotal.InLearningCount;

        }

        private void CalculateAdjustedDataLocks(ProviderPeriodEndSummaryModel model)
        {
            model.AdjustedDataLockedEarnings = model.TotalDataLockedEarnings - model.AlreadyPaidDataLockedEarnings;
            model.AdjustedDataLockedEarnings16To18 = model.TotalDataLockedEarnings16To18 - model.AlreadyPaidDataLockedEarnings16To18;
            model.AdjustedDataLockedEarnings19Plus = model.TotalDataLockedEarnings19Plus - model.AlreadyPaidDataLockedEarnings19Plus;
        }

        private void CalculateNegativeEarnings()
        {
            if (providerLearnerNegativeDcEarnings == null || providerLearnerNegativeDcEarnings.Count == 0) return;

            foreach (var learnerNegativeEarning in providerLearnerNegativeDcEarnings)
            {
                var learnerPayments = providerLearnerPayments?.Where(x => x.LearnerUln == learnerNegativeEarning.Uln && x.Total != 0m).ToList();
                var hasPayments = learnerPayments != null && learnerPayments.Count != 0;

                var learnerDataLocks = providerLearnerDataLockedEarnings?.Where(x => x.LearnerUln == learnerNegativeEarning.Uln && x.Total != 0m).ToList();
                var hasDataLocks = learnerDataLocks != null && learnerDataLocks.Count != 0;

                if (!hasPayments && !hasDataLocks) AddNegativeEarnings(learnerNegativeEarning);

                if (!hasPayments && hasDataLocks)
                {
                    AddNegativeEarnings(learnerNegativeEarning); 
                    AddNegativeDataLocks(learnerDataLocks);
                }
            }

            void AddNegativeEarnings(ProviderNegativeEarningsLearnerDcEarningAmounts learnerNegativeEarning)
            {
                switch (learnerNegativeEarning.ContractType)
                {
                    case ContractType.Act1:
                        negativeEarnings.ContractType1 = negativeEarnings.ContractType1.GetValueOrDefault() + learnerNegativeEarning.NegativeEarningsTotal;
                        break;

                    case ContractType.Act2:
                        negativeEarnings.ContractType2 = negativeEarnings.ContractType2.GetValueOrDefault() + learnerNegativeEarning.NegativeEarningsTotal;
                        break;
                }
            }

            void AddNegativeDataLocks(List<ProviderNegativeEarningsLearnerDataLockFundingLineTypeAmounts> learnerDataLocks)
            {
                negativeDataLocks.FundingLineType16To18Amount += learnerDataLocks.Sum(x => x.FundingLineType16To18Amount);
                negativeDataLocks.FundingLineType19PlusAmount += learnerDataLocks.Sum(x => x.FundingLineType19PlusAmount);
                negativeDataLocks.Total += learnerDataLocks.Sum(x => x.Total);
            }
        }

        private void ApplyNegativeEarningsAdjustments(ProviderPeriodEndSummaryModel providerSummary)
        {
            CalculateNegativeEarnings();
            AdjustDcEarnings();
            AdjustDataLockedEarnings();

            void AdjustDcEarnings()
            {
                providerSummary.DcEarnings.ContractType1 += (negativeEarnings?.ContractType1 ?? 0m);
                providerSummary.DcEarnings.ContractType2 += (negativeEarnings?.ContractType2 ?? 0m);
            }

            void AdjustDataLockedEarnings()
            {
                providerSummary.TotalDataLockedEarnings -= negativeDataLocks.Total;
                providerSummary.TotalDataLockedEarnings16To18 -= negativeDataLocks.FundingLineType16To18Amount;
                providerSummary.TotalDataLockedEarnings19Plus -= negativeDataLocks.FundingLineType19PlusAmount;
            }
        }
    }
}