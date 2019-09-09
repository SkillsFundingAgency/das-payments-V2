using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
   public static class FileHelpers
    {

        public static string CreateCsvFileName(short academicYear, byte collectionPeriod)
        {
            string fileName =
                $"Verification_{academicYear}_R{collectionPeriod:00}_{DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")}.csv";
            return fileName;
        }

        public static async Task UploadCsvFile(short academicYear, byte collectionPeriod, ISubmissionService submissionService,
            string csvString)
        {
            var fileName = FileHelpers.CreateCsvFileName(academicYear, collectionPeriod);

            var cbs = await submissionService.GetBlobStream(fileName, academicYear == 1920 ? "ILR1920" : "ILR1819");
            byte[] buffer = Encoding.UTF8.GetBytes(csvString);
            await cbs.WriteAsync(buffer, 0, buffer.Length);
            cbs.Close();
        }
    }
}
