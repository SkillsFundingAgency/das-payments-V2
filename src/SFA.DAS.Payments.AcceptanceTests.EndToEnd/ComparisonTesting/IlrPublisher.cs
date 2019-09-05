using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;

using ESFA.DC.JobStatus.Interface;
using Polly;
using SFA.DAS.Payments.AcceptanceTests.Services;
using SFA.DAS.Payments.AcceptanceTests.Services.Exceptions;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.ComparisonTesting
{
   public class IlrPublisher
    {
        private readonly IJobService jobService;
        private readonly IAzureStorageKeyValuePersistenceServiceConfig storageServiceConfig;
        public readonly IStreamableKeyValuePersistenceService storageService;

        public IlrPublisher(IJobService jobService, IAzureStorageKeyValuePersistenceServiceConfig storageServiceConfig, IStreamableKeyValuePersistenceService storageService)
        {
            this.jobService = jobService;
            this.storageServiceConfig = storageServiceConfig;
            this.storageService = storageService;
        }

        public async Task StoreAndPublishIlrFile(int ukprn, string ilrFileName, string ilrFile, int collectionYear, int collectionPeriod)
        {
            await StoreIlrFile(ukprn, ilrFileName, ilrFile);
            await PublishIlrFile(ukprn, ilrFileName, ilrFile, collectionYear, collectionPeriod);
        }


        private async Task PublishIlrFile(int ukprn, string ilrFileName, string ilrFile, int collectionYear, int collectionPeriod)
        {
            var submission = new SubmissionModel(ESFA.DC.Jobs.Model.Enums.EnumJobType.IlrSubmission, ukprn)
            {
                FileName = $"{ukprn}/{ilrFileName}",
                FileSizeBytes = ilrFile.Length,
                CreatedBy = "System",
                CollectionName = $"ILR{ilrFileName.Split('-')[2]}",
                Period = collectionPeriod,
                NotifyEmail = "dcttestemail@gmail.com",
                StorageReference = storageServiceConfig.ContainerName,
                CollectionYear = collectionYear
            };

            var jobId = await jobService.SubmitJob(submission);

            

            var retryPolicy = Policy
                .HandleResult<JobStatusType>(r => r != JobStatusType.Waiting)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var jobStatusResult = await retryPolicy.ExecuteAndCaptureAsync(async () => await jobService.GetJobStatus(jobId));
            if (jobStatusResult.Outcome == OutcomeType.Failure)
            {
                if (jobStatusResult.FinalHandledResult == JobStatusType.Ready)
                {
                    await jobService.DeleteJob(jobId);
                    throw new JobStatusNotWaitingException($"JobId:{jobId} is not yet in a Waiting Status. Current status: {jobStatusResult.FinalHandledResult}. " +
                                                           $"Ukprn: {ukprn} is probably blocked by an old job that hasn't completed.");
                }

                throw new JobStatusNotWaitingException($"JobId:{jobId} is not yet in a Waiting Status. Current status: {jobStatusResult.FinalHandledResult}");
            }

            await jobService.UpdateJobStatus(jobId, JobStatusType.Ready);

        }



        private async Task StoreIlrFile(int ukPrn, string ilrFileName, string ilrFile)
        {
            var byteArray = Encoding.UTF8.GetBytes(ilrFile);
            var stream = new MemoryStream(byteArray);

            var ilrStoragePathAndFileName = $"{ukPrn}/{ilrFileName}";

            await storageService.SaveAsync(ilrStoragePathAndFileName, stream);
        }

    }
}
