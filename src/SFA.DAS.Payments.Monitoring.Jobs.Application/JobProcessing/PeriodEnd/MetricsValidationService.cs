using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

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

            if (!result.IsSuccessStatusCode) return false;

            var content = await result.Content.ReadAsStringAsync();
            var submissionSummaryModel = JsonConvert.DeserializeObject<SubmissionsSummaryModel>(content);
            return submissionSummaryModel.IsWithinTolerance;
        }

        private string BuildUriFromParameters(long jobId, short academicYear, byte collectionPeriod)
        {
            return string.IsNullOrWhiteSpace(authCode)
                ? $"{Path.Combine(functionAddress, "/api/ValidateSubmissionWindow")}?jobId={jobId}&collectionPeriod={collectionPeriod}&AcademicYear={academicYear}"
                : $"{Path.Combine(functionAddress, "/api/ValidateSubmissionWindow")}?code={authCode}&jobId={jobId}&collectionPeriod={collectionPeriod}&AcademicYear={academicYear}";
        }
    }
}