using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain
{
    public static class Helpers
    {
        public static decimal GetPercentage(decimal amount, decimal total) => amount == total ? 100 : total > 0 ? (amount / total) * 100 : 0;

        public static decimal GetDefaultPercentage(this ContractTypeAmountsVerbose contactTypeAmountsVerbose)
        {
            var percentageContractType1 = contactTypeAmountsVerbose.PercentageContractType1;
            var percentageContractType2 = contactTypeAmountsVerbose.PercentageContractType2;
            return percentageContractType1 + percentageContractType2  == 0 ? 0 :
                (percentageContractType1 + percentageContractType2) / (percentageContractType1==0 ||  percentageContractType2==0 ? 1 : 2) ;
        }

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