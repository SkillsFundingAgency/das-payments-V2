using AutoMapper;
using DCT.TestDataGenerator.Model;
using ESFA.DC.ILR.TestDataGenerator.Interfaces;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.JobStatus.Interface;
using Polly;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Services.Exceptions;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Services.Intefaces
{
    public interface IIlrService
    {
        Task PublishNonLevyLearnerRequest(List<Training> currentIlr, string collectionPeriodText, SpecFlowContext context, Func<Task> WhenCheck);
    }

    public class NullIlrService : IIlrService
    {
        public Task PublishNonLevyLearnerRequest(List<Training> currentIlr, string collectionPeriodText, SpecFlowContext context, Func<Task> WhenCheck) => WhenCheck();
    }

    public class IlrService : IIlrService
    {
        private readonly IMapper mapper;
        private readonly ITdgService tdgService;
        private readonly TestSession testSession;
        private readonly IJobService jobService;
        private readonly IAzureStorageKeyValuePersistenceServiceConfig storageServiceConfig;
        private readonly IStreamableKeyValuePersistenceService storageService;

        public IlrService(IMapper mapper, ITdgService tdgService, TestSession testSession, IJobService jobService, IAzureStorageKeyValuePersistenceServiceConfig storageServiceConfig, IStreamableKeyValuePersistenceService storageService)
        {
            this.mapper = mapper;
            this.tdgService = tdgService;
            this.testSession = testSession;
            this.jobService = jobService;
            this.storageServiceConfig = storageServiceConfig;
            this.storageService = storageService;
        }

        public async Task PublishNonLevyLearnerRequest(List<Training> currentIlr, string collectionPeriodText, SpecFlowContext context, Func<Task> WhenCheck)
        {
            if (currentIlr?.Any() == false) return;

            var collectionYear = collectionPeriodText.ToDate().Year;
            var collectionPeriod = new CollectionPeriodBuilder().WithSpecDate(collectionPeriodText).Build().Period;
            var featureContext = (FeatureContext)context;
            var featureNumber = featureContext.FeatureInfo.Title.Substring(
                featureContext.FeatureInfo.Title.IndexOf("PV2-", StringComparison.Ordinal) + 4, 3);

            foreach (var ilr in currentIlr)
            {
                var learnerId = ilr.LearnerId;

                var nonLevyLearnerRequest = mapper.Map<NonLevyLearnerRequest>(ilr);
                nonLevyLearnerRequest.FeatureNumber = featureNumber;

                var ilrFile = await GenerateTestIlrFile(nonLevyLearnerRequest);

                RefreshTestSessionLearnerFromIlr(ilrFile.Value, learnerId);

                await WhenCheck();

                await StoreAndPublishIlrFile(learnerRequest: nonLevyLearnerRequest, ilrFileName: ilrFile.Key, ilrFile: ilrFile.Value, collectionYear: collectionYear, collectionPeriod: collectionPeriod);
            }
        }

        protected async Task<KeyValuePair<string, string>> GenerateTestIlrFile(LearnerRequestBase learnerRequest)
        {
            return await tdgService.GenerateIlrTestData((NonLevyLearnerRequest)learnerRequest);
        }

        protected void RefreshTestSessionLearnerFromIlr(string ilrFile, string learnerId = null)
        {
            XNamespace xsdns = "ESFA/ILR/2018-19";
            var xDoc = XDocument.Parse(ilrFile);
            var learner = xDoc.Descendants(xsdns + "Learner").First();
            var learningProvider = xDoc.Descendants(xsdns + "LearningProvider").First();

            var testSessionLearner = testSession.GetLearner(testSession.Provider.Ukprn, learnerId);

            testSessionLearner.Ukprn = long.Parse(learningProvider.Elements(xsdns + "UKPRN").First().Value);
            testSessionLearner.LearnRefNumber = learner.Elements(xsdns + "LearnRefNumber").First().Value;
            testSessionLearner.Uln = long.Parse(learner.Elements(xsdns + "ULN").First().Value);
        }

        protected async Task StoreAndPublishIlrFile(LearnerRequestBase learnerRequest, string ilrFileName, string ilrFile, int collectionYear, int collectionPeriod)
        {
            await StoreIlrFile(learnerRequest.Ukprn, ilrFileName, ilrFile);

            await PublishIlrFile(learnerRequest, ilrFileName, ilrFile, collectionYear, collectionPeriod);

        }

        private async Task PublishIlrFile(LearnerRequestBase learnerRequest, string ilrFileName, string ilrFile, int collectionYear, int collectionPeriod)
        {
            var nonLevyLearnerRequest = (NonLevyLearnerRequest)learnerRequest;

            var submission = new SubmissionModel(JobType.IlrSubmission, nonLevyLearnerRequest.Ukprn)
            {
                FileName = $"{learnerRequest.Ukprn}/{ilrFileName}",
                FileSizeBytes = ilrFile.Length,
                SubmittedBy = "System",
                CollectionName = $"ILR{ilrFileName.Split('-')[2]}",
                Period = collectionPeriod,
                NotifyEmail = "SpecFlow@e2e.com",
                StorageReference = storageServiceConfig.ContainerName,
                CollectionYear = collectionYear
            };

            var jobId = await jobService.SubmitJob(submission);

            //TODO: Overriding JobId, but better implementation needed. Eg: calling GetProvider with proper Identifier when needed.
            foreach (var provider in testSession.Providers)
            {
                provider.JobId = jobId;
            }

            var retryPolicy = Policy
                .Handle<JobStatusNotWaitingException>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            await retryPolicy.ExecuteAsync(async () =>
            {
                var jobStatus = await jobService.GetJobStatus(jobId);
                if (jobStatus != JobStatusType.Waiting)
                    throw new JobStatusNotWaitingException($"JobId:{jobId} is not yet in a Waiting Status");
            }
            );

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
