using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface ISubmissionWindowValidationClient
    {
        Task<bool> Validate(long jobId, short academicYear, byte collectionPeriod);
    }
    
    public class SubmissionWindowValidationClient : ISubmissionWindowValidationClient
    {
        private readonly string authCode;
        private readonly Uri functionAddressUri;

        public SubmissionWindowValidationClient(string authCode, string functionAddress)
        {
            this.authCode = authCode;
            functionAddressUri = new Uri(functionAddress);
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
                ? $"{new Uri(functionAddressUri, "/api/ValidateSubmissionWindow")}?jobId={jobId}&collectionPeriod={collectionPeriod}&AcademicYear={academicYear}"
                : $"{new Uri(functionAddressUri, "/api/ValidateSubmissionWindow")}?code={authCode}&jobId={jobId}&collectionPeriod={collectionPeriod}&AcademicYear={academicYear}";
        }
    }
}