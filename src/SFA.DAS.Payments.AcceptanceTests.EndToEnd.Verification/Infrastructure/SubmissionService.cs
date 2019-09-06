using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public interface ISubmissionService
    {
        Task<IEnumerable<string>> ImportPlaylist();

        Task<IEnumerable<string>> CreateTestFiles(IEnumerable<string> playlist);

        Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> filelist);

        Task DeleteFiles(IEnumerable<string> filelist);
    }

    public class SubmissionService : ISubmissionService
    {
        private static CloudBlobClient blobClient;

        private readonly IJsonSerializationService serializationService;

        private readonly CloudStorageSettings cloudStorageSettings;

        public SubmissionService(IAzureStorageKeyValuePersistenceServiceConfig storageServiceConfig, IJsonSerializationService serializationService, CloudStorageSettings cloudStorageSettings)
        {
            this.serializationService = serializationService;
            this.cloudStorageSettings = cloudStorageSettings;
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
            throw new NotImplementedException();

        }

        //private async Task<FileUploadJob> DoSubmitJob(string file, int delay)
        //{
        //    long ukprn = long.Parse(row.Cells["UKPRN"].Value.ToString());
        //    var filename = row.Cells["Filename"].Value.ToString();
        //    var collectionType = row.Cells["Type"].Value.ToString();

        //    int period = 8;

        //    await Task.Delay(delay);

        //    var jobid = await _submissionService.SubmitJob(
        //        row.Cells["Filename"].Value.ToString(),
        //        decimal.Parse(row.Cells["Filesize"].Value.ToString()),
        //        System.Security.Principal.WindowsIdentity.GetCurrent().Name,
        //        ukprn,
        //        collectionType,
        //        period,
        //        false,
        //        "dcttestemail@gmail.com"
        //        , _submissionService.ContainerName(collectionType));

        //    var status = await _submissionService.GetJob(ukprn, jobid);
        //    bool secondStageRequired = true;
        //    // this line will resubmit for second stage processing
        //    status = await JobStatusCompletionState(ukprn, jobid, secondStageRequired);
        //    if (secondStageRequired && status.Status == JobStatusType.Waiting)
        //    {
        //        row.Cells["Status"].Value = "Proc. 2nd stage";
        //        secondStageRequired = false;
        //        status = await JobStatusCompletionState(ukprn, jobid, secondStageRequired);
        //    }
        //    DataGridViewHelper.SetCellStyleAndValue(row.Cells["Status"], status.Status);

        //    ++_uiProgress.Value;
        //    row.Cells["End"].Value = DateTime.UtcNow;
        //    try
        //    {
        //        DateTime endTime = status.DateTimeUpdatedUtc.Value;
        //        row.Cells["End"].Value = endTime;
        //        row.Cells["Duration"].Value = endTime - status.DateTimeSubmittedUtc.Value;
        //    }
        //    catch
        //    { }
        //    //            row.Cells["Learners"].Value = status.TotalLearners;
        //}

        public async Task DeleteFiles(IEnumerable<string> filelist)
        {
            throw new NotImplementedException();
        }


        public void DeleteFiles()
        {
            // iterate through list of files and delete
        }

        private async void AddFileRow(string fp, bool selected, string collectionType)
        {
            //int rowid = _uiFiles.Rows.Add();
            //var row = _uiFiles.Rows[rowid];
            //row.Cells["Filename"].Value = fp;
            //row.Cells["UKPRN"].Value = UkprnFromFilename(fp);
            //row.Cells["Submit"].Value = false;
            //row.Cells["Filesize"].Value = await GetBlobFilesize(fp, collectionType);
            //row.Cells["Submit"].Value = selected;
            //row.Cells["Type"].Value = collectionType;
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

        public string ContainerName(string collectionType)
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

        private async Task<bool> Exists(string fileName, string containerName)
        {
            var cloudBlobContainer = blobClient.GetContainerReference(ContainerName(containerName));
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            return await cloudBlockBlob.ExistsAsync();
        }

        internal async Task<long> GetBlobFilesize(string fp, string containerName)
        {
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(ContainerName(containerName));
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
