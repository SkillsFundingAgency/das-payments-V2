using System;
using System.Collections.Generic;
using System.Linq;
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
                           DateTime testStartDateTime,
                           DateTime testEndDateTime,
                           Action<decimal?> verificationAction);

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
          //await submissionService.ClearPaymentsData(playlist);
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
            DateTime testStartDateTime, DateTime testEndDateTime, Action<decimal?> verificationAction)
        {
            var resultsList = results.ToList();
            var ukprnList = resultsList.Select(r => r.Ukprn).ToList();

            byte collectionPeriod = (byte)resultsList.FirstOrDefault().PeriodNumber;


            var groupedResults = resultsList.GroupBy(g => g.CollectionYear);

            foreach (var groupedResult in groupedResults)
            {
                short academicYear = (short)groupedResult.Key;

                await ExtractAndSavePaymentsData(testStartDateTime, testEndDateTime, academicYear, collectionPeriod);

                await ExtractAndSaveDatastoreData(academicYear, collectionPeriod, ukprnList);

                decimal? totalMissingRequiredPayments = await verificationService.GetTotalMissingRequiredPayments(academicYear, collectionPeriod, true,
                    testStartDateTime,
                    testEndDateTime);

                decimal? totalEarningYtd = await verificationService.GetTotalEarningsYtd(academicYear, collectionPeriod, ukprnList);

                var actualPercentage = totalMissingRequiredPayments / totalEarningYtd * 100;

                verificationAction.Invoke(actualPercentage);
            }
        }

        private async Task ExtractAndSaveDatastoreData(short academicYear, byte collectionPeriod, List<long> ukprnList)
        {
            var secondDataCsv = await verificationService.GetDataStoreCsv(academicYear, collectionPeriod, ukprnList);

            //publish the csv.
            await FileHelpers.UploadCsvFile(FileHelpers.ReportType.DataStore,
                                            academicYear,
                                            collectionPeriod,
                                            submissionService,
                                            secondDataCsv);
        }

        private async Task ExtractAndSavePaymentsData(DateTime testStartDateTime, DateTime testEndDateTime, short academicYear, byte collectionPeriod)
        {
            string csvString = await verificationService.GetVerificationDataCsv(academicYear,
                                                                                collectionPeriod,
                                                                                true,
                                                                                testStartDateTime,
                                                                                testEndDateTime);

            //publish the csv.
            await FileHelpers.UploadCsvFile(FileHelpers.ReportType.PaymentsData,
                                            academicYear,
                                            collectionPeriod,
                                            submissionService,
                                            csvString);
        }

        public async Task<DateTimeOffset?> GetNewDateTime(List<long> ukprns)
        {
            return await verificationService.GetLastActivityDate(ukprns);
        }
    }
}