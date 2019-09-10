using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model;
using Polly;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure;
using SFA.DAS.Payments.AcceptanceTests.Services;
using SFA.DAS.Payments.AcceptanceTests.Services.Exceptions;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.ComparisonTesting
{
    public interface ITestOrchestrator {
        Task<IEnumerable<string>> SetupTestFiles();

        Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> filelist);

        Task DeleteTestFiles(IEnumerable<string> filelist);

        Task VerifyResults(IEnumerable<FileUploadJob> results,
            DateTime testStartDateTime, DateTime testEndDateTime, Action<decimal?> verificationAction);
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
            // Import file list from playlist.json in cloud storage
          var playlist = await submissionService.ImportPlaylist();
            // Create new file from existing one
          return await submissionService.CreateTestFiles(playlist);
        }

        public Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> filelist)
        {
            return submissionService.SubmitFiles(filelist);
        }

        public Task DeleteTestFiles(IEnumerable<string> filelist)
        {
            return submissionService.DeleteFiles(filelist);
        }

        public async Task VerifyResults(IEnumerable<FileUploadJob> results,
            DateTime testStartDateTime, DateTime testEndDateTime, Action<decimal?> verificationAction)
        {
            byte collectionPeriod = (byte)results.FirstOrDefault().PeriodNumber;


            var groupedResults = results.ToList().GroupBy(g => g.CollectionYear);

            foreach (var groupedResult in groupedResults)
            {
                short academicYear = (short)groupedResult.Key;

                string csvString = await verificationService.GetVerificationDataCsv(academicYear, collectionPeriod,
                    true,
                    testStartDateTime,
                    testEndDateTime);

                //publish the csv.
                await FileHelpers.UploadCsvFile(FileHelpers.ReportType.PaymentsData, academicYear, collectionPeriod,
                    submissionService, csvString);

                var secondDataCsv = await verificationService.GetDataStoreCsv(academicYear, collectionPeriod);

                //publish the csv.
                await FileHelpers.UploadCsvFile(FileHelpers.ReportType.DataStore, academicYear, collectionPeriod,
                    submissionService, secondDataCsv);

                decimal? actualPercentage = await verificationService.GetTheNumber(academicYear, collectionPeriod, true,
                    testStartDateTime,
                    testEndDateTime);

                verificationAction.Invoke(actualPercentage);
            }
        }
    }
}
