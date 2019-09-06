using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SFA.DAS.Payments.AcceptanceTests.Services;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public interface ISubmissionService
    {
        Task<IEnumerable<string>> ImportPlaylist();

        Task<IEnumerable<string>> CreateTestFiles(IEnumerable<string> playlist);

        Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> filelist);

        Task DeleteFiles(IEnumerable<string> filelist);

        Task<CloudBlobStream> GetBlobStream(string fileName, string containerName);

    }

    public class SubmissionService : ISubmissionService
    {
        private static CloudBlobClient blobClient;

        private readonly IJsonSerializationService serializationService;

        private readonly CloudStorageSettings cloudStorageSettings;

        private readonly IJobService jobService;

        public SubmissionService(IAzureStorageKeyValuePersistenceServiceConfig storageServiceConfig, IJsonSerializationService serializationService, CloudStorageSettings cloudStorageSettings, IJobService jobService)
        {
            this.serializationService = serializationService;
            this.cloudStorageSettings = cloudStorageSettings;
            this.jobService = jobService;
            var cloudStorageAccount = CloudStorageAccount.Parse(storageServiceConfig.ConnectionString);
            blobClient = cloudStorageAccount.CreateCloudBlobClient();
        }

        public async Task<IEnumerable<string>> ImportPlaylist()
        {
            var blobContainer = blobClient.GetContainerReference("control-files");
            var blob = blobContainer.GetBlockBlobReference("playlist.json");
            var text = await blob.DownloadTextAsync();
            return serializationService.Deserialize<List<string>>(text);
        }

        public async Task<IEnumerable<string>> CreateTestFiles(IEnumerable<string> playlist)
        {
            var newFileList = new List<string>();
            foreach (var file in playlist)
            {
                var collectionType = GetCollectionTypeFromFilename(file);
                newFileList.Add(await CopyBlobFileToNewILRFile(file, collectionType));
            }

            return newFileList;
        }

        public async Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> filelist)
        {
            List<Task<FileUploadJob>> _outstandingSubmissionTasks = new List<Task<FileUploadJob>>();
            var _totalJobsToSubmit = 0;
            var _submissionDelay = 8000;

            foreach (var file in filelist)
            {
                _outstandingSubmissionTasks.Add(DoSubmitJob(file, _submissionDelay * _totalJobsToSubmit++));
            }

            return await Task.WhenAll(_outstandingSubmissionTasks);
        }

        private async Task<FileUploadJob> DoSubmitJob(string file, int delay)
        {
            long ukprn = UkprnFromFilename(file);
            var collectionType = GetCollectionTypeFromFilename(file);

            int period = 8;

            await Task.Delay(delay);

            var submission = new SubmissionModel(EnumJobType.IlrSubmission, ukprn)
                             {
                                 FileName = $"{file}",
                                 FileSizeBytes = await GetBlobFilesize(file,ContainerName(collectionType)),
                                 CreatedBy = "System",
                                 CollectionName = collectionType,
                                 Period = period,
                                 IsFirstStage = true,
                                 NotifyEmail = "dcttestemail@gmail.com",
                                 StorageReference = ContainerName(collectionType)
                             };

            var jobId = await jobService.SubmitJob(submission);
            var status = await jobService.GetJob(ukprn, jobId);
            // this line will resubmit for second stage processing
            bool secondStageRequired = true;
            status = await JobStatusCompletionState(ukprn, jobId, secondStageRequired);
            if ( secondStageRequired && status.Status == JobStatusType.Waiting)
            {
                secondStageRequired = false;
                status = await JobStatusCompletionState(ukprn, jobId, secondStageRequired);
            }

            return status;
        }

        private async Task<FileUploadJob> JobStatusCompletionState(long ukprn, long jobId, bool secondStageRequired)
        {
            bool completed = false;
            FileUploadJob result = new FileUploadJob();
            while (!completed)
            {
                var status = await jobService.GetJob(ukprn, jobId);
                if (status.Status == JobStatusType.Waiting ||
                    status.Status == JobStatusType.Completed)
                {
                    completed = true;
                    result = status;
                    if (secondStageRequired)
                    {
                        await jobService.UpdateJobStatus(jobId, JobStatusType.Ready);
                    }
                }
                else if (status.Status == JobStatusType.Failed ||
                         status.Status == JobStatusType.FailedRetry)
                {
                    completed = true;
                    result = status;
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
            return result;
        }

        public async Task DeleteFiles(IEnumerable<string> filelist)
        {
            var jobList = new List<Task>();
            foreach (var file in filelist)
            {
                var blobContainer = blobClient.GetContainerReference(ContainerName(GetCollectionTypeFromFilename(file)));
                var blockBlob = blobContainer.GetBlockBlobReference(file);
                jobList.Add(blockBlob.DeleteIfExistsAsync());
            }


            await Task.WhenAll(jobList);
        }

        private long UkprnFromFilename(string fp)
        {
            if (fp.Length == 49 && fp.Substring(9, 4) == "ILR-")
            {
                long result = long.Parse(fp.Substring(0, 8));
                return result;
            }
            else if (fp.Length == 40 && fp.Substring(0, 4) == "ILR-")
            {
                long result = long.Parse(fp.Substring(4, 8));
                return result;
            }
            return 0;
        }

        private string GetCollectionTypeFromFilename(string fp)
        {
            if (fp.Length == 49 && fp.Substring(9, 4) == "ILR-")
            {
                string result = $"ILR{fp.Substring(22, 4)}";
                return result;
            }
            else if (fp.Length == 40 && fp.Substring(0, 4) == "ILR-")
            {
                string result = $"ILR{fp.Substring(13, 4)}";
                return result;
            }
            return $"{fp.Length}";
        }

        private string ContainerName(string collectionType)
        {
            switch (collectionType)
            {
                case "ILR1819":
                    return cloudStorageSettings.Ilr1819ContainerName;
                case "ILR1920":
                    return cloudStorageSettings.ILR1920ContainerName;
                default:
                    throw new ArgumentOutOfRangeException($"The collection type {collectionType} doesn't have a containerName configured");
            }
        }

        private async Task CopyBlob(string source, string target, string containerName)
        {
            var cloudBlobContainer = blobClient.GetContainerReference(ContainerName(containerName));
            var sourceBlob = cloudBlobContainer.GetBlockBlobReference(source);
            var targetBlob = cloudBlobContainer.GetBlockBlobReference(target);
            bool ok = false;
            while (!ok)
            {
                var result = await targetBlob.StartCopyAsync(sourceBlob);
                targetBlob.FetchAttributes();
                while (targetBlob.CopyState.Status == CopyStatus.Pending)
                {
                    await Task.Delay(500);
                    targetBlob.FetchAttributes();
                }
                ok = targetBlob.CopyState.Status == CopyStatus.Success;
            }
        }



        public async Task<CloudBlobStream> GetBlobStream(string fileName, string containerName)
        {
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(ContainerName(containerName));
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            return await cloudBlockBlob.OpenWriteAsync();
        }


        private async Task<bool> Exists(string fileName, string containerName)
        {
            var cloudBlobContainer = blobClient.GetContainerReference(ContainerName(containerName));
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            return await cloudBlockBlob.ExistsAsync();
        }

        private async Task<long> GetBlobFilesize(string fp, string containerName)
        {
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(containerName);
            var blob = cloudBlobContainer.GetBlockBlobReference(fp);
            if (blob.Exists())
            {
                blob.FetchAttributes();
                return blob.Properties.Length;
            }
            return 0;
        }

        private async Task<string> CopyBlobFileToNewILRFile(string filename, string type)
        {
            string newFilename = filename;
            newFilename = IncrementILRFormatNameByASecond(filename);
            var fileok = !await Exists(newFilename, type);
            while (!fileok)
            {
                newFilename = IncrementILRFormatNameByASecond(newFilename);
                fileok = !await Exists(newFilename, type);
            }
            await CopyBlob(filename, newFilename, type);
            return newFilename;
        }

        private static string IncrementILRFormatNameByASecond(string filename)
        {
            string newFilename;
            string timepart = filename.Substring(filename.Length - 13, 6);
            TimeSpan hours = TimeSpan.FromHours(int.Parse(timepart.Substring(0, 2)));
            TimeSpan minutes = TimeSpan.FromMinutes(int.Parse(timepart.Substring(2, 2)));
            TimeSpan seconds = TimeSpan.FromSeconds(int.Parse(timepart.Substring(4, 2)));
            TimeSpan total = hours + minutes + seconds;
            total += TimeSpan.FromSeconds(1);
            timepart = total.ToString("hhmmss");
            newFilename = filename.Substring(0, filename.Length - 13);
            newFilename += timepart;
            newFilename += filename.Substring(filename.Length - 7);
            return newFilename;
        }
    }
}
