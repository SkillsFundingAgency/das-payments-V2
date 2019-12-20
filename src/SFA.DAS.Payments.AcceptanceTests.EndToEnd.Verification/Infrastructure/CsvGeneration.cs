using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    internal class CsvGeneration
    {
        private readonly MetricsCalculator metricsCalculator;


        public CsvGeneration(MetricsCalculator metricsCalculator)
        {
            this.metricsCalculator = metricsCalculator;
        }


        public string BuildCsvString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CreateEarningsSummarySection());
            sb.Append(Environment.NewLine);
            sb.Append(CreatePaymentsSummarySection());
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(CreateDataLockSummarySection());
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(CreateTotalsSection());


            return sb.ToString();
        }

        private string CreateTotalsSection()
        {
           


            return ",Required Payments,Actual Payments,DC Earnings,DAS Earnings" +
                   Environment.NewLine +
                   "Accounted for money," +
                   $"\"{metricsCalculator.AccountedForRequiredPayments:C}\"," +
                   $"\"{metricsCalculator.PaymentsValues.TotalPaymentsYtd:C}\"," +
                   $"\"{metricsCalculator.AccountedForDcEarnings:P}\"," +
                   $"\"{metricsCalculator.AccountedForDasEarnings:P}\"," +
                   Environment.NewLine +
                   "Accounted not for money," +
                   $"\"{metricsCalculator.NotAccountedForRequiredPayments:C}\"," +
                   $"\"{metricsCalculator.NotAccountedForActualPayments:C}\"";
        }

        private string CreateDataLockSummarySection()
        {
            return "DataLock Amount,Datalocked Payments,Adjusted Datalocks" +
                   Environment.NewLine +
                   $"\"{metricsCalculator.PaymentsValues.DataLockedEarnings:C}\"," +
                   $"\"{metricsCalculator.PaymentsValues.DataLockedPayments:C}\"," +
                   $"\"{metricsCalculator.PaymentsValues.AdjustedDataLocks:C}\",";
        }

        private string CreatePaymentsSummarySection()
        {
            return
                "Required Payments made this month,Payments made before this month YTD,Expected Payments YTD after running Period End,Total payments this month,Total ACT 1 payments YTD,Total ACT 2 payments YTD,Total payments YTD,Held Back Completion Payments" +
                Environment.NewLine +
                $"\"{metricsCalculator.PaymentsValues.RequiredPaymentsThisMonth:C}\"," +
                $"\"{metricsCalculator.PaymentsValues.PaymentsPriorToThisMonthYtd:C}\"," +
                $"\"{metricsCalculator.PaymentsValues.ExpectedPaymentsAfterPeriodEnd:C}\"," +
                $"\"{metricsCalculator.PaymentsValues.TotalPaymentsThisMonth:C}\"," +
                $"\"{metricsCalculator.PaymentsValues.TotalAct1Ytd:C}\"," +
                $"\"{metricsCalculator.PaymentsValues.TotalAct2Ytd:C}\"," +
                $"\"{metricsCalculator.PaymentsValues.TotalPaymentsYtd:C}\","+
                $"\"{metricsCalculator.PaymentsValues.HeldBackCompletionThisMonth:C}\"";
        }

        private string CreateEarningsSummarySection()
        {
            return
                $"\"DAS Earnings\",\"DC Earnings\",\"DAS Earnings v DC Earnings\"{Environment.NewLine}" +
                $"\"{metricsCalculator.PaymentsValues?.DasEarnings:C}\",\"{metricsCalculator.DcValues.Total:C}\",\"{metricsCalculator.DasEarningVsDcEarnings:P}\"";
        }
    }
}