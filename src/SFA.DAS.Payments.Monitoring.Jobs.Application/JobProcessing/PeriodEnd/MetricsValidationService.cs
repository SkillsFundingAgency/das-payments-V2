using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public class MetricsValidationService : IMetricsValidationService
    {
        private readonly string authCode;
        private readonly string functionAddress;

        public MetricsValidationService(string authCode, string functionAddress)
        {
            this.authCode = authCode;
            this.functionAddress = functionAddress;
        }

        public async Task<bool> Validate(long jobId, short academicYear, byte collectionPeriod)
        {
            var result = await new HttpClient().GetAsync(BuildUriFromParameters(jobId, academicYear, collectionPeriod));
            return result.StatusCode == HttpStatusCode.OK;
        }

        private string BuildUriFromParameters(long jobId, short academicYear, byte collectionPeriod)
        {
            return string.IsNullOrWhiteSpace(authCode)
                ? $"{functionAddress}?jobId={jobId}&collectionPeriod={collectionPeriod}&AcademicYear={academicYear}"
                : $"{functionAddress}?code={authCode}&jobId={jobId}&collectionPeriod={collectionPeriod}&AcademicYear={academicYear}";
        }
    }
}