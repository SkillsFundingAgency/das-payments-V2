using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public interface ITestOrchestrator
    {
        Task<IEnumerable<string>> SetupTestFiles();

        Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> fileList);

        Task DeleteTestFiles(IEnumerable<string> fileList);

        Task VerifyResults(IEnumerable<FileUploadJob> results,
                           DateTimeOffset testStartDateTime,
                           DateTimeOffset testEndDateTime,
                           Action<decimal?, decimal, decimal?> verificationAction);

        Task<DateTimeOffset?> GetNewDateTime(List<long> ukprns);
    }

    public class TestOrchestrator : ITestOrchestrator
    {
        private readonly ISubmissionService submissionService;
        private readonly IVerificationService verificationService;

        public TestOrchestrator(ISubmissionService submissionService, IVerificationService verificationService)
        {
            this.submissionService = submissionService;
            this.verificationService = verificationService;
        }

        public async Task<IEnumerable<string>> SetupTestFiles()
        {
          var playlist = await submissionService.ImportPlaylist();
          await submissionService.ClearPaymentsData(playlist);
          return await submissionService.CreateTestFiles(playlist);
        }

        public Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> fileList)
        {
            return submissionService.SubmitFiles(fileList);
        }

        public Task DeleteTestFiles(IEnumerable<string> fileList)
        {
            return submissionService.DeleteFiles(fileList);
        }

        public async Task VerifyResults(IEnumerable<FileUploadJob> results,
            DateTimeOffset testStartDateTime, DateTimeOffset testEndDateTime, Action<decimal?, decimal, decimal?> verificationAction)
        {
            var resultsList = results.ToList();
            var ukprnList = resultsList.Select(r => r.Ukprn).ToList();

            byte collectionPeriod = (byte) resultsList.FirstOrDefault().PeriodNumber;


            var groupedResults = resultsList.GroupBy(g => g.CollectionYear);

            foreach (var groupedResult in groupedResults)
            {
                short academicYear = (short) groupedResult.Key;

                var paymentCsv =
                    await ExtractPaymentsData(testStartDateTime, testEndDateTime, academicYear, collectionPeriod);

                var dataStoreCsv = await ExtractDataStoreData(academicYear, collectionPeriod, ukprnList);

               var paymentTotals = await verificationService.GetPaymentTotals(
                    academicYear, collectionPeriod, true,
                    testStartDateTime,
                    testEndDateTime);

                decimal? totalEarningYtd =
                    await verificationService.GetTotalEarningsYtd(academicYear, collectionPeriod, ukprnList);

                var settings = await submissionService.ReadSettingsFile();
                decimal tolerance = settings.Tolerance;

                decimal? actualPercentage = null;
                if (totalEarningYtd != 0)
                {
                    actualPercentage = paymentTotals?.missingPayments / totalEarningYtd * 100;
                }

                var summaryCsv = CreateSummaryCsv(actualPercentage, tolerance, totalEarningYtd, paymentTotals);
                var queryTimeWindowCsv = CreateQueryTimeWindowCsv(testStartDateTime, testEndDateTime);

                await SaveCsv(paymentCsv, dataStoreCsv, summaryCsv, queryTimeWindowCsv,  academicYear, collectionPeriod);

                var earningsDifference = totalEarningYtd - paymentTotals?.earningsYtd;

                verificationAction.Invoke(actualPercentage, tolerance, earningsDifference);
            }
        }

        private string CreateQueryTimeWindowCsv(DateTimeOffset testStartDateTime, DateTimeOffset testEndDateTime)
        {
            var header = "Query Start Time, Query End Time, Duration";
            var row = $"{testStartDateTime:O},{testEndDateTime:O},{(testEndDateTime - testStartDateTime):G}";
            return $"{header}{Environment.NewLine}{row}";
        }

        private async Task SaveCsv(string paymentCsv, string dataStoreCsv, string summaryCsv, string queryTimeWindow,
            short academicYear,
            byte collectionPeriod)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Summary");
            sb.Append(summaryCsv);
            sb.AppendLine();
            sb.AppendLine("Earnings");
            sb.Append(dataStoreCsv);
            sb.AppendLine();
            sb.AppendLine("Payments");
            sb.Append(paymentCsv);
            sb.AppendLine();
            sb.AppendLine("Query Time Window");
            sb.Append(queryTimeWindow);

            await FileHelpers.UploadCsvFile(academicYear, collectionPeriod, submissionService, sb.ToString());
        }

        private string CreateSummaryCsv(decimal? actualPercentage, decimal tolerance, decimal? totalEarningYtd, (decimal? missingPayments, decimal? earningsYtd)? paymentTotals)
        {
            var header = "Difference, Tolerance, Earnings (YTD) - DC, Missing Required Payments, Earnings (YTD) - DAS";
            var row = $"{actualPercentage},{tolerance},{totalEarningYtd},{paymentTotals?.missingPayments}, {paymentTotals?.earningsYtd}";
            return $"{header}{Environment.NewLine}{row}";
        }

        private async Task<string> ExtractDataStoreData(short academicYear, byte collectionPeriod, List<long> ukprnList)
        {
            return await verificationService.GetEarningsCsv(academicYear, collectionPeriod, ukprnList);
        }

        private async Task<string> ExtractPaymentsData(DateTimeOffset testStartDateTime, DateTimeOffset testEndDateTime, short academicYear, byte collectionPeriod)
        {
            return await verificationService.GetPaymentsDataCsv(academicYear, collectionPeriod,
                true,
                testStartDateTime,
                testEndDateTime);
        }

        public async Task<DateTimeOffset?> GetNewDateTime(List<long> ukprns)
        {
            return await verificationService.GetLastActivityDate(ukprns);
        }
    }
}