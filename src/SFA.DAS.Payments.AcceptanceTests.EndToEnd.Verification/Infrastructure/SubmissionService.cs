using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Services;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public interface ISubmissionService
    {
        Task<IEnumerable<string>> ImportPlaylist();

        Task<IEnumerable<string>> CreateTestFiles(IEnumerable<string> playlist);

        Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> fileList);

        Task DeleteFiles(IEnumerable<string> fileList);

        Task<CloudBlobStream> GetResultsBlobStream(string fileName);

        Task ClearPaymentsData(IEnumerable<string> playlist);

        Task<TestSettings> ReadSettingsFile();
    }

    public class SubmissionService : ISubmissionService
    {
        private const string ResultsContainerName = "results";
        private const string ControlFileContainerName = "control-files";

        private const string SettingFile = "settings.json";

        private static CloudBlobClient blobClient;

        private readonly IJobService jobService;

        private readonly TestPaymentsDataContext paymentsContext;

        private readonly Configuration configuration;

        private readonly IJsonSerializationService serializationService;

        public SubmissionService(Configuration configuration,
                                 IJsonSerializationService serializationService,
                                 IJobService jobService,
                                 TestPaymentsDataContext paymentsContext)
        {
            this.configuration = configuration;
            this.serializationService = serializationService;
            this.jobService = jobService;
            this.paymentsContext = paymentsContext;
            var cloudStorageAccount = CloudStorageAccount.Parse(configuration.DcStorageConnectionString);
            blobClient = cloudStorageAccount.CreateCloudBlobClient();
        }

        public async Task<IEnumerable<string>> ImportPlaylist()
        {
            var settings = await ReadSettingsFile();
            var blobContainer = blobClient.GetContainerReference(ControlFileContainerName);
            var blob = blobContainer.GetBlockBlobReference(settings.Playlist);
            var text = await blob.DownloadTextAsync();
            return serializationService.Deserialize<List<string>>(text);
        }

        public async Task<IEnumerable<string>> CreateTestFiles(IEnumerable<string> playlist)
        {
            var newFileList = new List<string>();
            foreach (var file in playlist)
            {
                var collectionType = GetCollectionTypeFromFilename(file);
                newFileList.Add(await CopyBlobFileToNewIlrFile(file, collectionType));
            }

            return newFileList;
        }

        public async Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> fileList)
        {
            var totalJobsToSubmit = 0;
            var submissionDelay = 8000;
            var outstandingSubmissionTasks = fileList.Select(file => DoSubmitJob(file, submissionDelay * totalJobsToSubmit++)).ToList();
            return await Task.WhenAll(outstandingSubmissionTasks);
        }

        public async Task DeleteFiles(IEnumerable<string> fileList)
        {
            var jobList = new List<Task>();
            foreach (var file in fileList)
            {
                var blobContainer = blobClient.GetContainerReference(ContainerName(GetCollectionTypeFromFilename(file)));
                var blockBlob = blobContainer.GetBlockBlobReference(file);
                jobList.Add(blockBlob.DeleteIfExistsAsync());
            }

            await Task.WhenAll(jobList);
        }

        public async Task<CloudBlobStream> GetResultsBlobStream(string fileName)
        {
            var cloudBlobContainer = blobClient.GetContainerReference(ResultsContainerName);
            await cloudBlobContainer.CreateIfNotExistsAsync();
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
            
            return await cloudBlockBlob.OpenWriteAsync();
        }

        public async Task ClearPaymentsData(IEnumerable<string> playlist)
        {
            if (configuration.ClearPaymentsData)
            {
                foreach (var file in playlist)
                {
                    var ukprn = UkprnFromFilename(file);
                    await paymentsContext.ClearPaymentsDataAsync(ukprn);
                }
            }
            else
            {
                await Task.CompletedTask;
            }
        }

        public async Task<TestSettings> ReadSettingsFile()
        {
            var blobContainer = blobClient.GetContainerReference(ControlFileContainerName);
            var blob = blobContainer.GetBlockBlobReference(SettingFile);
            var text = await blob.DownloadTextAsync();
            return serializationService.Deserialize<TestSettings>(text);
        }

        private async Task<FileUploadJob> DoSubmitJob(string file, int delay)
        {
            var ukprn = UkprnFromFilename(file);
            var collectionType = GetCollectionTypeFromFilename(file);

            var settings = await ReadSettingsFile();
            var period = settings.Period;

            await Task.Delay(delay);

            var submission = new SubmissionModel(EnumJobType.IlrSubmission, ukprn)
                             {
                                 FileName = $"{file}",
                                 FileSizeBytes = GetBlobFileSize(file, ContainerName(collectionType)),
                                 CreatedBy = "System",
                                 CollectionName = collectionType,
                                 Period = period,
                                 IsFirstStage = true,
                                 NotifyEmail = "dcttestemail@gmail.com",
                                 StorageReference = ContainerName(collectionType)
                             };

            var jobId = await jobService.SubmitJob(submission);
            await jobService.GetJob(ukprn, jobId);

            var status = await JobStatusCompletionState(ukprn, jobId, secondStageRequired: true);
            if (status.Status == JobStatusType.Waiting)
            {
                status = await JobStatusCompletionState(ukprn, jobId, secondStageRequired: false);
            }

            return status;
        }

        private async Task<FileUploadJob> JobStatusCompletionState(long ukprn, long jobId, bool secondStageRequired)
        {
            var completed = false;
            var result = new FileUploadJob();
            while (!completed)
            {
                var status = await jobService.GetJob(ukprn, jobId);
                if (status.Status == JobStatusType.Waiting ||
                    status.Status == JobStatusType.Completed)
                {
                    completed = true;
                    result = status;
                    if (secondStageRequired) await jobService.UpdateJobStatus(jobId, JobStatusType.Ready);
                }
                else if (status.Status == JobStatusType.Failed ||
                         status.Status == JobStatusType.FailedRetry)
                {
                    completed = true;
                    result = status;
                }
                else
                {
                    await Task.Delay(configuration.DcJobEventCheckDelay);
                }
            }

            return result;
        }

        private long UkprnFromFilename(string fp)
        {
            if (fp.Length == 49 && fp.Substring(9, 4) == "ILR-")
            {
                var result = long.Parse(fp.Substring(0, 8));


                return result;
            }

            if (fp.Length == 40 && fp.Substring(0, 4) == "ILR-")
            {
                var result = long.Parse(fp.Substring(4, 8));


                return result;
            }

            return 0;
        }

        private string GetCollectionTypeFromFilename(string fp)
        {
            if (fp.Length == 49 && fp.Substring(9, 4) == "ILR-")
            {
                var result = $"ILR{fp.Substring(22, 4)}";


                return result;
            }

            if (fp.Length == 40 && fp.Substring(0, 4) == "ILR-")
            {
                var result = $"ILR{fp.Substring(13, 4)}";


                return result;
            }

            return $"{fp.Length}";
        }

        private string ContainerName(string collectionType)
        {
            switch (collectionType)
            {
                case "ILR1819":
                    return configuration.Ilr1819ContainerName;
                case "ILR1920":
                    return configuration.Ilr1920ContainerName;
                default:
                    throw new ArgumentOutOfRangeException($"The collection type {collectionType} doesn't have a containerName configured");
            }
        }

        private async Task CopyBlob(string source, string target, string containerName)
        {
            var cloudBlobContainer = blobClient.GetContainerReference(ContainerName(containerName));
            var sourceBlob = cloudBlobContainer.GetBlockBlobReference(source);
            var targetBlob = cloudBlobContainer.GetBlockBlobReference(target);
            var ok = false;
            while (!ok)
            {
                await targetBlob.StartCopyAsync(sourceBlob);
                targetBlob.FetchAttributes();
                while (targetBlob.CopyState.Status == CopyStatus.Pending)
                {
                    await Task.Delay(500);
                    targetBlob.FetchAttributes();
                }

                ok = targetBlob.CopyState.Status == CopyStatus.Success;
            }
        }

        private async Task<bool> Exists(string fileName, string containerName)
        {
            var cloudBlobContainer = blobClient.GetContainerReference(ContainerName(containerName));
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
            return await cloudBlockBlob.ExistsAsync();
        }

        private async Task<string> CopyBlobFileToNewIlrFile(string filename, string type)
        {
            var newFilename = IncrementIlrFormatNameByASecond(filename);
            var fileok = !await Exists(newFilename, type);
            while (!fileok)
            {
                newFilename = IncrementIlrFormatNameByASecond(newFilename);
                fileok = !await Exists(newFilename, type);
            }

            await CopyBlob(filename, newFilename, type);
            return newFilename;
        }

        private static long GetBlobFileSize(string fp, string containerName)
        {
            var cloudBlobContainer = blobClient.GetContainerReference(containerName);
            var blob = cloudBlobContainer.GetBlockBlobReference(fp);
            if (blob.Exists())
            {
                blob.FetchAttributes();
                return blob.Properties.Length;
            }

            return 0;
        }

        private static string IncrementIlrFormatNameByASecond(string filename)
        {
            var timepart = filename.Substring(filename.Length - 13, 6);
            var hours = TimeSpan.FromHours(int.Parse(timepart.Substring(0, 2)));
            var minutes = TimeSpan.FromMinutes(int.Parse(timepart.Substring(2, 2)));
            var seconds = TimeSpan.FromSeconds(int.Parse(timepart.Substring(4, 2)));
            var total = hours + minutes + seconds;
            total += TimeSpan.FromSeconds(1);
            timepart = total.ToString("hhmmss");
            var newFilename = filename.Substring(0, filename.Length - 13);
            newFilename += timepart;
            newFilename += filename.Substring(filename.Length - 7);
            return newFilename;
        }
    }
}