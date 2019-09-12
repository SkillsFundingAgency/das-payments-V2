using System;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public static class FileHelpers
    {
        private static string CreateCsvFileName(short academicYear, byte collectionPeriod)
        {
            var fileName =
                $"Verification_{academicYear}_R{collectionPeriod:00}_{DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")}.csv";
            return fileName;
        }

        public static async Task UploadCsvFile(short academicYear,
                                               byte collectionPeriod,
                                               ISubmissionService submissionService,
                                               string csvString)
        {
            var fileName = CreateCsvFileName(academicYear, collectionPeriod);

            var cbs = await submissionService.GetBlobStream(fileName, academicYear == 1920 ? "ILR1920" : "ILR1819");
            var buffer = Encoding.UTF8.GetBytes(csvString);
            await cbs.WriteAsync(buffer, 0, buffer.Length);
            cbs.Close();
        }
    }
}