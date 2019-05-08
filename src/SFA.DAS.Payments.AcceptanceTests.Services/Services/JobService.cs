namespace SFA.DAS.Payments.AcceptanceTests.Services
{
    using System.Configuration;
    using System.Threading.Tasks;
    using ESFA.DC.Jobs.Model;
    using ESFA.DC.Jobs.Model.Enums;
    using ESFA.DC.JobStatus.Interface;
    using SFA.DAS.Payments.AcceptanceTests.Services.BespokeHttpClient;
    using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;
    using System;
    using Newtonsoft.Json;
    using SFA.DAS.Payments.AcceptanceTests.Services.Dtos;

    public class JobService : IJobService
    {
        private readonly IBespokeHttpClient _httpClient;
        private string _apiBaseUrl => ConfigurationManager.AppSettings["apiBaseUrl"];

        public JobService(IBespokeHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JobStatusType> GetJobStatus(long jobId)
        {
            var url = $"{_apiBaseUrl}/job/{jobId}/status";
            var data = await _httpClient.GetDataAsync(url);
            return JsonConvert.DeserializeObject<JobStatusType>(data);
        }

        public async Task<string> UpdateJobStatus(long jobId, JobStatusType status)
        {
            var job = new JobStatusDto()
            {
                JobId = jobId,
                JobStatus = status,
                NumberOfLearners =  0
            };
            return await _httpClient.SendDataAsync($"{_apiBaseUrl}/job/status", job);
        }

        public async Task<long> SubmitJob(SubmissionModel submissionMessage)
        {
            if (string.IsNullOrEmpty(submissionMessage?.FileName))
            {
                throw new ArgumentException("submission message should have file name");
            }

            var job = new FileUploadJob()
            {
                Ukprn = submissionMessage.Ukprn,
                DateTimeSubmittedUtc = DateTime.UtcNow,
                Priority = 1,
                Status = JobStatusType.Ready,
                SubmittedBy = submissionMessage.SubmittedBy,
                FileName = submissionMessage.FileName,
                IsFirstStage = true,
                StorageReference = submissionMessage.StorageReference,
                FileSize = submissionMessage.FileSizeBytes,
                CollectionName = submissionMessage.CollectionName,
                PeriodNumber = submissionMessage.Period,
                NotifyEmail = submissionMessage.NotifyEmail,
                JobType = submissionMessage.JobType,
                TermsAccepted = submissionMessage.JobType == JobType.EasSubmission ? true : (bool?)null,
                CollectionYear = submissionMessage.CollectionYear
            };

            var response = await _httpClient.SendDataAsync($"{_apiBaseUrl}/job", job);
            long.TryParse(response, out var result);

            return result;
        }
    }
}
