﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using ESFA.DC.ILR.TestDataGenerator.Interfaces;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.JobStatus.Interface;
using MoreLinq;
using Polly;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Core.Services;
using SFA.DAS.Payments.AcceptanceTests.Services;
using SFA.DAS.Payments.AcceptanceTests.Services.Exceptions;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class IlrDcService : IIlrService
    {
        private readonly ITdgService tdgService;
        private readonly TestSession testSession;
        private readonly IJobService jobService;
        private readonly IAzureStorageKeyValuePersistenceServiceConfig storageServiceConfig;
        private readonly IStreamableKeyValuePersistenceService storageService;
        private readonly IPaymentsDataContext dataContext;

        public IlrDcService(ITdgService tdgService, TestSession testSession, IJobService jobService, IAzureStorageKeyValuePersistenceServiceConfig storageServiceConfig, IStreamableKeyValuePersistenceService storageService, IPaymentsDataContext dataContext)
        {
            this.tdgService = tdgService;
            this.testSession = testSession;
            this.jobService = jobService;
            this.storageServiceConfig = storageServiceConfig;
            this.storageService = storageService;
            this.dataContext = dataContext;
        }

        public async Task PublishLearnerRequest(List<Training> previousIlr, List<Training> currentIlr, List<Learner> learners, string collectionPeriodText, string featureNumber, Func<Task> clearCache)
        {
            var collectionYear = collectionPeriodText.ToDate().Year;
            var collectionPeriod = new CollectionPeriodBuilder().WithSpecDate(collectionPeriodText).Build().Period;

            if (currentIlr != null && currentIlr.Any())
            {
                // convert to TestSession.Learners
                learners = new List<Learner>();

                learners.AddRange(currentIlr.DistinctBy(ilr => ilr.LearnerId).Select(dist => new Learner()
                {
                    Ukprn = dist.Ukprn, Uln = dist.Uln, LearnerIdentifier = dist.LearnerId,
                    PostcodePrior = dist.PostcodePrior, EefCode = dist.EefCode,
                    EmploymentStatusMonitoring = CreateLearnerEmploymentStatusMonitoringFromTraining(previousIlr?.Single(p=>p.LearnerId == dist.LearnerId), dist)
                }));

                foreach (var learner in learners)
                {
                    CreateAimsForIlrLearner(learner, currentIlr.SingleOrDefault(c=>c.LearnerId == learner.LearnerIdentifier));
                }
            }

            var learnerMutator = LearnerMutatorFactory.Create(featureNumber, learners);

            var ilrFile = await tdgService.GenerateIlrTestData(learnerMutator, (int)testSession.Provider.Ukprn);

            await RefreshTestSessionLearnerFromIlr(ilrFile.Value, learners);

            // this needs to be called here as the LearnRefNumber is updated to match the ILR in RefreshTestSessionLearnerFromIlr above
            await clearCache();

            await StoreAndPublishIlrFile((int)testSession.Provider.Ukprn, ilrFileName: ilrFile.Key, ilrFile: ilrFile.Value, collectionYear: collectionYear, collectionPeriod: collectionPeriod);
        }

        private List<EmploymentStatusMonitoring> CreateLearnerEmploymentStatusMonitoringFromTraining(Training previousIlr, Training currentIlr)
        {
            var employmentStatusMonitoringList = new List<EmploymentStatusMonitoring>();
            if (!string.IsNullOrWhiteSpace(previousIlr?.Employer) || !string.IsNullOrWhiteSpace(previousIlr?.SmallEmployer))
            {
                employmentStatusMonitoringList.Add(new EmploymentStatusMonitoring()
                                                   {
                                                       LearnerId = previousIlr.LearnerId,
                                                       EmploymentStatusApplies = !string.IsNullOrWhiteSpace(previousIlr.EmploymentStatusApplies) ? previousIlr.EmploymentStatusApplies : previousIlr.StartDate.ToDate().AddMonths(-6).ToString(),
                                                       EmploymentStatus = !string.IsNullOrWhiteSpace(previousIlr.EmploymentStatus) ? previousIlr.EmploymentStatus : "in paid employment",
                                                       Employer = previousIlr.Employer,
                                                       SmallEmployer = previousIlr.SmallEmployer
                                                   });
            }

            if (previousIlr?.EmploymentStatusApplies != currentIlr.EmploymentStatusApplies)
            {
                employmentStatusMonitoringList.Add(new EmploymentStatusMonitoring()
                {
                    LearnerId = currentIlr.LearnerId,
                    EmploymentStatusApplies = currentIlr.EmploymentStatusApplies,
                    EmploymentStatus = currentIlr.EmploymentStatus,
                    Employer = currentIlr.Employer,
                    SmallEmployer = currentIlr.SmallEmployer
                });
            }

            return employmentStatusMonitoringList;
        }

        private void CreateAimsForIlrLearner(Learner learner, Training currentIlr)
        {
            var aim = new Aim(currentIlr);
            aim.PriceEpisodes.Add(new Price()
            {
                TotalTrainingPriceEffectiveDate = currentIlr.TotalTrainingPriceEffectiveDate,
                TotalTrainingPrice = currentIlr.TotalTrainingPrice,
                TotalAssessmentPriceEffectiveDate = currentIlr.TotalAssessmentPriceEffectiveDate,
                TotalAssessmentPrice = currentIlr.TotalAssessmentPrice,
                ContractType = currentIlr.ContractType,
                AimSequenceNumber = currentIlr.AimSequenceNumber,
                SfaContributionPercentage = currentIlr.SfaContributionPercentage,
                CompletionHoldBackExemptionCode = currentIlr.CompletionHoldBackExemptionCode,
                Pmr = currentIlr.Pmr
            });

            learner.Aims.Add(aim);
        }

        private async Task RefreshTestSessionLearnerFromIlr(string ilrFile, IEnumerable<Learner> learners)
        {
            XNamespace xsdns = tdgService.IlrNamespace;
            var xDoc = XDocument.Parse(ilrFile);
            var learnerDescendants = xDoc.Descendants(xsdns + "Learner");

            for (var i = 0; i < learners.Count(); i++)
            {
                var request = learners.Skip(i).Take(1).First();
                var testSessionLearner = testSession.GetLearner(testSession.Provider.Ukprn, request.LearnerIdentifier);
                var originalUln = testSessionLearner.Uln;
                var learner = learnerDescendants.Skip(i).Take(1).First();
                testSessionLearner.LearnRefNumber = learner.Elements(xsdns + "LearnRefNumber").First().Value;
                testSessionLearner.Uln = long.Parse(learner.Elements(xsdns + "ULN").First().Value);

                await UpdatePaymentHistoryTables(testSessionLearner.Ukprn, originalUln, testSessionLearner.Uln,
                    testSessionLearner.LearnRefNumber);
            }
        }

        private async Task UpdatePaymentHistoryTables(long ukprn, long originalUln, long newUln, string learnRefNumber)
        {
            var payments = dataContext.Payment.Where(p => p.Ukprn == ukprn && p.LearnerUln == originalUln);
            foreach (var payment in payments)
            {
                payment.LearnerReferenceNumber = learnRefNumber;
                payment.LearnerUln = newUln;
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
                NotifyEmail = "dcttestemail@gmail.com",
                StorageReference = storageServiceConfig.ContainerName,
                CollectionYear = collectionYear
            };

            var jobId = await jobService.SubmitJob(submission);

            //TODO: Overriding JobId, but better implementation needed. Eg: calling GetProvider with proper Identifier when needed.
            foreach (var provider in testSession.Providers.Where(x => x.Ukprn == ukprn))
            {
                provider.JobId = jobId;
            }

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
