using System;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain
{
    public static class Helpers
    {
        public static decimal GetPercentage(decimal amount, decimal total) => Math.Round(amount == total ? 100 : total > 0 ? (amount / total) * 100 : 0, 2);

        public static ContractTypeAmountsVerbose CreatePaymentMetrics(IPeriodEndSummaryModel summaryModel)
        {
            var paymentMetrics = new ContractTypeAmountsVerbose()
            {
                ContractType1 = summaryModel.YearToDatePayments.ContractType1 +
                                summaryModel.Payments.ContractType1 +
                                summaryModel.AdjustedDataLockedEarnings +
                                summaryModel.HeldBackCompletionPayments.ContractType1,
                ContractType2 = summaryModel.YearToDatePayments.ContractType2 +
                                summaryModel.Payments.ContractType2 +
                                summaryModel.HeldBackCompletionPayments.ContractType2
            };
            paymentMetrics.DifferenceContractType1 =
                paymentMetrics.ContractType1 - summaryModel.DcEarnings.ContractType1;
            paymentMetrics.DifferenceContractType2 =
                paymentMetrics.ContractType2 - summaryModel.DcEarnings.ContractType2;
            paymentMetrics.PercentageContractType1 = Helpers.GetPercentage(paymentMetrics.ContractType1, summaryModel.DcEarnings.ContractType1);
            paymentMetrics.PercentageContractType2 = Helpers.GetPercentage(paymentMetrics.ContractType2, summaryModel.DcEarnings.ContractType2);
            paymentMetrics.Percentage = Helpers.GetPercentage(paymentMetrics.Total, summaryModel.DcEarnings.Total);
            return paymentMetrics;
        }
    }
}