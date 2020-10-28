using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function.AcceptanceTests.Helpers
{
    public static class ValidateSubmissionWindowTriggerHelper
    {
        public static async Task<HttpStatusCode> Trigger(long jobId, short academicYear, byte collectionPeriod)
        {
            var result = await new HttpClient().GetAsync(
                $"http://localhost:7071/api/ValidateSubmissionWindow?jobid={jobId}&collectionPeriod={collectionPeriod}&AcademicYear={academicYear}"
            );
            return result.StatusCode;
        }
    }
}
