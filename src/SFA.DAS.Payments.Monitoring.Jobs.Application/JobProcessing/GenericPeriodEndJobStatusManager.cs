using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public interface IGenericPeriodEndJobStatusManager : IJobStatusManager
    {
    }

    public class GenericPeriodEndJobStatusManager : JobStatusManager, IGenericPeriodEndJobStatusManager
    {
        public GenericPeriodEndJobStatusManager(IPaymentLogger logger, IUnitOfWorkScopeFactory scopeFactory,
            IJobServiceConfiguration configuration) : base(logger, scopeFactory, configuration)
        {
        }

        public override IJobStatusService GetJobStatusService(IUnitOfWorkScope scope)
        {
            return scope.Resolve<IGenericPeriodEndJobStatusService>();
        }

        public override async Task<List<long>> GetCurrentJobs(IJobStorageService jobStorage)
        {
            return await jobStorage.GetCurrentPeriodEndExcludingStartJobs(cancellationToken);
        }

        public async void StartMonitoringArchivePipeline(long jobId, JobType jobType)
        {
            // Set variables
            var tenantID = "<your tenant ID>";
            var applicationId = "<your application ID>";
            var authenticationKey = "<your authentication key for the application>";
            var subscriptionId = "12f72527-6622-45d3-90a4-0a5d3644c45c";
            var resourceGroup = "DCOL-DAS-DataFactoryDAS-WEU";
            var region = "West Europe";
            var dataFactoryName =
                "DCOL-DAS-DataFactoryDAS-WEU";
            var storageAccount = "<your storage account name to copy data>";
            var storageKey = "<your storage account key>";
            // specify the container and input folder from which all files 
            // need to be copied to the output folder. 
            var inputBlobPath =
                "<path to existing blob(s) to copy data from, e.g. containername/inputdir>";
            //specify the contains and output folder where the files are copied
            var outputBlobPath =
                "<the blob path to copy data to, e.g. containername/outputdir>";

            // name of the Azure Storage linked service, blob dataset, and the pipeline
            var storageLinkedServiceName = "AzureStorageLinkedService";
            var blobDatasetName = "BlobDataset";
            var pipelineName = "CopyPaymentsToArchive";

            // Authenticate and create a data factory management client
            var app = ConfidentialClientApplicationBuilder.Create(applicationId)
                .WithAuthority("https://login.microsoftonline.com/" + tenantID)
                .WithClientSecret(authenticationKey)
                .WithLegacyCacheCompatibility(false)
                .WithCacheOptions(CacheOptions.EnableSharedCacheOptions)
                .Build();

            var result = await app.AcquireTokenForClient(
                    new[] { "https://management.azure.com//.default" })
                .ExecuteAsync();
            ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);

            var client = new DataFactoryManagementClient(cred)
            {
                SubscriptionId = subscriptionId
            };

            // Create a pipeline run
            Console.WriteLine("Creating pipeline run...");
            var parameters = new Dictionary<string, object>
            {
                { "inputPath", inputBlobPath },
                { "outputPath", outputBlobPath }
            };
            var runResponse = client.Pipelines.CreateRunWithHttpMessagesAsync(
                resourceGroup, dataFactoryName, pipelineName, parameters: parameters
            ).Result.Body;
            Console.WriteLine("Pipeline run ID: " + runResponse.RunId);

            // Monitor the pipeline run
            Console.WriteLine("Checking pipeline run status...");
            PipelineRun pipelineRun;
            while (true)
            {
                pipelineRun = client.PipelineRuns.Get(resourceGroup, dataFactoryName, runResponse.RunId);
                Console.WriteLine("Status: " + pipelineRun.Status);
                if (pipelineRun.Status == "InProgress" || pipelineRun.Status == "Queued")
                    Thread.Sleep(15000);
                else
                    break;
            }

            // Check the copy activity run details

            var filterParams = new RunFilterParameters(
                DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(10));
            var queryResponse = client.ActivityRuns.QueryByPipelineRun(
                resourceGroup, dataFactoryName, runResponse.RunId, filterParams);
            if (pipelineRun.Status == "Succeeded")
                Console.WriteLine(queryResponse.Value.First().Output);
            else
                Console.WriteLine(queryResponse.Value.First().Error);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}