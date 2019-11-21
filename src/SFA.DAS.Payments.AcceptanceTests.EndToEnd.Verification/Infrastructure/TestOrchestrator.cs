using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public interface ITestOrchestrator
    {
        Task<IEnumerable<string>> SetupTestFiles();

        Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> fileList);

        Task DeleteTestFiles(IEnumerable<string> fileList);

        Task VerifyResults(IEnumerable<FileUploadJob> results,
                           Action<decimal?, decimal, decimal?> verificationAction);

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

        public async Task VerifyResults(IEnumerable<FileUploadJob> results, Action<decimal?, decimal, decimal?> verificationAction)
        {
            var resultsList = results.ToList();
            var ukprnList = resultsList.Select(r => r.Ukprn).ToList();
            byte collectionPeriod = (byte) resultsList.FirstOrDefault().PeriodNumber;

            var groupedResults = resultsList.GroupBy(g => g.CollectionYear);

            foreach (var groupedResult in groupedResults)
            {
                short academicYear = (short) groupedResult.Key;

                var dasPaymentsData = await ExtractPaymentsData(results.Min(r=>r.DateTimeSubmittedUtc.Value), academicYear, collectionPeriod, ukprnList);
                var dcEarningsYtd = await ExtractDataStoreData(academicYear, collectionPeriod, ukprnList);

                MetricsCalculator metricsCalculator = new MetricsCalculator(dasPaymentsData, dcEarningsYtd);

                var totalDcEarningsYtd = metricsCalculator.DcValues.Total;
                var dasEarningsYtd =  metricsCalculator.PaymentsValues.DasEarnings;


                var settings = await submissionService.ReadSettingsFile();
                decimal tolerance = settings.Tolerance;

                decimal? actualPercentage = null;
                if (dasEarningsYtd != 0)
                {
                    actualPercentage = metricsCalculator.NotAccountedForRequiredPayments / dasEarningsYtd * 100;
                }

                var csv = new CsvGeneration(metricsCalculator).BuildCsvString();

                await SaveCsv(csv,  academicYear, collectionPeriod);

                var earningsDifference = totalDcEarningsYtd - dasEarningsYtd;

                verificationAction.Invoke(actualPercentage, tolerance, earningsDifference);
            }
        }

        private async Task SaveCsv(string csvString,short academicYear,byte collectionPeriod)
        {
            await FileHelpers.UploadCsvFile(academicYear, collectionPeriod, submissionService, csvString);
        }

        private async Task<DcValues> ExtractDataStoreData(short academicYear, byte collectionPeriod, List<long> ukprnList)
        {
            return await verificationService.GetDcEarningsData(academicYear, collectionPeriod, ukprnList);
        }

        private async Task<PaymentsValues> ExtractPaymentsData(DateTime runStartDateTime,
            short academicYear, byte collectionPeriod, IList<long> ukprnList)
        {
            return await verificationService.GetPaymentsData(runStartDateTime, academicYear, collectionPeriod,
                true,
                ukprnList);
        }
    }
}