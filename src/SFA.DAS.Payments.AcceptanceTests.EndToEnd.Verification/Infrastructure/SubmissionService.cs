using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.ComparisonTesting
{
    public class SubmissionService
    {
        private dynamic _cloudStorageSettings;

        private dynamic _serializationService;

        public void ImportFiles()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("content-files");
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference("playlist.json");
            var text = blob.DownloadText(); // Read from Playlist.json file in storage account
            var files = _serializationService.Deserialize<List<string>>(text);
            foreach (var file in files)
            {
                var collectionType = GetCollectionTypeFromFilename(file);
                AddFileRow(file, true, collectionType);
            }
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
            switch (collectionType) {
                case "ILR1819":
                    return _cloudStorageSettings.ILR1819ContainerName;
                case "ILR1920":
                    return _cloudStorageSettings.ILR1920ContainerName;
                default:
                    throw new ArgumentOutOfRangeException($"The collection type {collectionType} doesn't have a containerName configured");
            }
        }

        public async void CopyBlob(string source, string target, string containerName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(ContainerName(containerName));
            CloudBlockBlob sourceBlob = cloudBlobContainer.GetBlockBlobReference(source);
            CloudBlockBlob targetBlob = cloudBlobContainer.GetBlockBlobReference(target);
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

        public async Task<bool> Exists(string fileName, string containerName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(ContainerName(containerName));
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            return await cloudBlockBlob.ExistsAsync();
        }

        internal async Task<long> GetBlobFilesize(string fp, string containerName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_cloudStorageSettings.ConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(ContainerName(containerName));
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
            CopyBlob(filename, newFilename, type);
            return newFilename;
        }

        internal static string IncrementILRFormatNameByASecond(string filename)
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
