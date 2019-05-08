using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.AcceptanceTests.Services.Intefaces
{
    using AutoMapper;
    using ESFA.DC.ILR.TestDataGenerator.Interfaces;
    using ESFA.DC.IO.AzureStorage.Config.Interfaces;
    using ESFA.DC.IO.Interfaces;
    using ESFA.DC.Jobs.Model.Enums;
    using ESFA.DC.JobStatus.Interface;
    using Polly;
    using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
    using SFA.DAS.Payments.AcceptanceTests.Core.Data;
    using SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators;
    using SFA.DAS.Payments.AcceptanceTests.Services.Exceptions;
    using SFA.DAS.Payments.Application.Repositories;
    using SFA.DAS.Payments.Tests.Core;
    using SFA.DAS.Payments.Tests.Core.Builders;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class IlrDcService : IIlrService
    {
        private readonly IMapper mapper;
        private readonly ITdgService tdgService;
        private readonly TestSession testSession;
        private readonly IJobService jobService;
        private readonly IAzureStorageKeyValuePersistenceServiceConfig storageServiceConfig;
        private readonly IStreamableKeyValuePersistenceService storageService;
        private readonly IPaymentsDataContext dataContext;

        public IlrDcService(IMapper mapper, ITdgService tdgService, TestSession testSession, IJobService jobService, IAzureStorageKeyValuePersistenceServiceConfig storageServiceConfig, IStreamableKeyValuePersistenceService storageService, IPaymentsDataContext dataContext)
        {
            this.mapper = mapper;
            this.tdgService = tdgService;
            this.testSession = testSession;
            this.jobService = jobService;
            this.storageServiceConfig = storageServiceConfig;
            this.storageService = storageService;
            this.dataContext = dataContext;
        }

        public async Task PublishLearnerRequest(List<Training> currentIlr, string collectionPeriodText, string featureNumber, Func<Task> verifyIlr)
        {
            if (currentIlr?.Any() == false) return;

            var collectionYear = collectionPeriodText.ToDate().Year;
            var collectionPeriod = new CollectionPeriodBuilder().WithSpecDate(collectionPeriodText).Build().Period;

            var learnerRequests = mapper.Map<IEnumerable<LearnerRequest>>(currentIlr);
            var learnerMutator = LearnerMutatorFactory.Create(featureNumber, learnerRequests);

            var ilrFile = await tdgService.GenerateIlrTestData(learnerMutator, (int)testSession.Provider.Ukprn);

            await RefreshTestSessionLearnerFromIlr(ilrFile.Value, learnerRequests);

            await verifyIlr();

            await StoreAndPublishIlrFile((int)testSession.Provider.Ukprn, ilrFileName: ilrFile.Key, ilrFile: ilrFile.Value, collectionYear: collectionYear, collectionPeriod: collectionPeriod);
        }

        private async Task RefreshTestSessionLearnerFromIlr(string ilrFile, IEnumerable<LearnerRequest> currentIlr)
        {
            XNamespace xsdns = tdgService.IlrNamespace;
            var xDoc = XDocument.Parse(ilrFile);
            var learners = xDoc.Descendants(xsdns + "Learner");

            for (var i = 0; i < currentIlr.Count(); i++)
            {
                var request = currentIlr.Skip(i).Take(1).First();
                var testSessionLearner = testSession.GetLearner(testSession.Provider.Ukprn, request.LearnerId);
                var learner = learners.Skip(i).Take(1).First();
                testSessionLearner.LearnRefNumber = learner.Elements(xsdns + "LearnRefNumber").First().Value;
                testSessionLearner.Uln = long.Parse(learner.Elements(xsdns + "ULN").First().Value);

                await UpdatePaymentHistoryTables(testSessionLearner.Ukprn, testSessionLearner.LearnRefNumber,
                    testSessionLearner.Uln);
            }
        }

        private async Task UpdatePaymentHistoryTables(long ukprn, string learnRefNumber, long uln)
        {
            var payments = dataContext.Payment.Where(p => p.Ukprn == ukprn);
            foreach (var payment in payments)
            {
                payment.LearnerReferenceNumber = learnRefNumber;
                payment.LearnerUln = uln;
            }

            await dataContext.SaveChangesAsync();
        }

        private async Task StoreAndPublishIlrFile(int ukprn, string ilrFileName, string ilrFile, int collectionYear, int collectionPeriod)
        {
            await StoreIlrFile(ukprn, ilrFileName, ilrFile);

            await PublishIlrFile(ukprn, ilrFileName, ilrFile, collectionYear, collectionPeriod);

        }

        private async Task PublishIlrFile(int ukprn, string ilrFileName, string ilrFile, int collectionYear, int collectionPeriod)
        {
            var submission = new SubmissionModel(JobType.IlrSubmission, ukprn)
            {
                FileName = $"{ukprn}/{ilrFileName}",
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

        private Task StoreIlrFile(int ukPrn, string ilrFileName, string ilrFile)
        {
            var byteArray = Encoding.UTF8.GetBytes(ilrFile);
            var stream = new MemoryStream(byteArray);

            var ilrStoragePathAndFileName = $"{ukPrn}/{ilrFileName}";

            return storageService.SaveAsync(ilrStoragePathAndFileName, stream);
        }
    }
}
