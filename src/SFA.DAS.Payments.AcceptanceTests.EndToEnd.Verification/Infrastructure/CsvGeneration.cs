using System;
using System.Globalization;
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
                   $"\"{metricsCalculator.AccountedForRequiredPayments.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"," +
                   $"\"{metricsCalculator.PaymentsValues.TotalPaymentsYtd.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"," +
                   $"\"{metricsCalculator.AccountedForDcEarnings:P}\"," +
                   $"\"{metricsCalculator.AccountedForDasEarnings:P}\"," +
                   Environment.NewLine +
                   "Accounted not for money," +
                   $"\"{metricsCalculator.NotAccountedForRequiredPayments.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"," +
                   $"\"{metricsCalculator.NotAccountedForActualPayments.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"";
        }

        private string CreateDataLockSummarySection()
        {
            return "DataLock Amount,Datalocked Payments,Adjusted Datalocks" +
                   Environment.NewLine +
                   $"\"{metricsCalculator.PaymentsValues.DataLockedEarnings.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"," +
                   $"\"{metricsCalculator.PaymentsValues.DataLockedPayments.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"," +
                   $"\"{metricsCalculator.PaymentsValues.AdjustedDataLocks.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\",";
        }

        private string CreatePaymentsSummarySection()
        {
            return
                "Required Payments made this month,Payments made before this month YTD,Expected Payments YTD after running Period End,Total payments this month,Total ACT 1 payments YTD,Total ACT 2 payments YTD,Total payments YTD,Held Back Completion Payments" +
                Environment.NewLine +
                $"\"{metricsCalculator.PaymentsValues.RequiredPaymentsThisMonth.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"," +
                $"\"{metricsCalculator.PaymentsValues.PaymentsPriorToThisMonthYtd.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"," +
                $"\"{metricsCalculator.PaymentsValues.ExpectedPaymentsAfterPeriodEnd.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"," +
                $"\"{metricsCalculator.PaymentsValues.TotalPaymentsThisMonth.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"," +
                $"\"{metricsCalculator.PaymentsValues.TotalAct1Ytd.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"," +
                $"\"{metricsCalculator.PaymentsValues.TotalAct2Ytd.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"," +
                $"\"{metricsCalculator.PaymentsValues.TotalPaymentsYtd.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\","+
                $"\"{metricsCalculator.PaymentsValues.HeldBackCompletionThisMonth.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\"";
        }

        private string CreateEarningsSummarySection()
        {
            return
                $"\"DAS Earnings\",\"DC Earnings\",\"DAS Earnings v DC Earnings\"{Environment.NewLine}" +
                $"\"{metricsCalculator.PaymentsValues?.DasEarnings.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\",\"{metricsCalculator.DcValues.Total.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))}\",\"{metricsCalculator.DasEarningVsDcEarnings:P}\"";
        }
    }
}