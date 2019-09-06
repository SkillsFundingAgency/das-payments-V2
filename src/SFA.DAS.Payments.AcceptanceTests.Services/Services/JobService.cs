using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using Newtonsoft.Json;
using SFA.DAS.Payments.AcceptanceTests.Services.BespokeHttpClient;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;
using JobStatusDto = ESFA.DC.Jobs.Model.JobStatusDto;

namespace SFA.DAS.Payments.AcceptanceTests.Services
{
    public class JobService : IJobService
    {
        private readonly IBespokeHttpClient httpClient;

        public JobService(IBespokeHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<JobStatusType> GetJobStatus(long jobId)
        {
            var data = await httpClient.GetDataAsync($"job/{jobId}/status");
            return JsonConvert.DeserializeObject<JobStatusType>(data);
        }

        public async Task<string> UpdateJobStatus(long jobId, JobStatusType status)
        {
            var job = new JobStatusDto()
            {
                JobId = jobId,
                JobStatus = (int)status,
                NumberOfLearners =  0
            };
            return await httpClient.SendDataAsync("job/status", job);
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
                DateTimeCreatedUtc = DateTime.UtcNow,
                Priority = 1,
                Status = JobStatusType.Ready,
                CreatedBy = submissionMessage.CreatedBy,
                FileName = submissionMessage.FileName,
                IsFirstStage = submissionMessage.IsFirstStage,
                StorageReference = submissionMessage.StorageReference,
                FileSize = submissionMessage.FileSizeBytes,
                CollectionName = submissionMessage.CollectionName,
                PeriodNumber = submissionMessage.Period,
                NotifyEmail = submissionMessage.NotifyEmail,
                TermsAccepted = submissionMessage.JobType == EnumJobType.EasSubmission ? true : (bool?)null,
                CollectionYear = submissionMessage.CollectionYear
            };

            var response = await httpClient.SendDataAsync("job", job);
            long.TryParse(response, out var result);

            return result;
        }

        public async Task DeleteJob(long jobId)
        {
            await httpClient.DeleteAsync($"job/{jobId}");
        }

        public async Task<IEnumerable<long>> GetJobsByStatus(int ukprn, params int[] status)
        {
            var data = await httpClient.GetDataAsync($"job/{ukprn}").ConfigureAwait(false);
            var jobList = JsonConvert.DeserializeObject<IEnumerable<FileUploadJob>>(data);
            return jobList.Where(x => status.Contains((int) x.Status)).Select(j => j.JobId);
        }

        public async Task<FileUploadJob> GetJob(long ukprn, long jobId)
        {
            var data = await httpClient.GetDataAsync($"job/{ukprn}/{jobId}");
            return JsonConvert.DeserializeObject<FileUploadJob>(data);
        }
    }
}
