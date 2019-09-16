using System;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public static class FileHelpers
    {
        public enum ReportType
        {
            PaymentsData,

            DataStore
        }

        public static string CreateCsvFileName(short academicYear, byte collectionPeriod, ReportType reportType)
        {
            var fileName =
                $"Verification_{reportType}_{academicYear}_R{collectionPeriod:00}_{DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")}.csv";
            return fileName;
        }

        public static async Task UploadCsvFile(ReportType reportType,
                                               short academicYear,
                                               byte collectionPeriod,
                                               ISubmissionService submissionService,
                                               string csvString)
        {
            var fileName = CreateCsvFileName(academicYear, collectionPeriod, reportType);

            var cbs = await submissionService.GetBlobStream(fileName, academicYear == 1920 ? "ILR1920" : "ILR1819");
            var buffer = Encoding.UTF8.GetBytes(csvString);
            await cbs.WriteAsync(buffer, 0, buffer.Length);
            cbs.Close();
        }
    }
}