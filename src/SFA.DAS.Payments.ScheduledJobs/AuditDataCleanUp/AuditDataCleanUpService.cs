using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration;

namespace SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp
{
    public class AuditDataCleanUpService : IAuditDataCleanUpService
    {
        private readonly IPaymentsDataContext dataContext;
        private readonly IEndpointInstanceFactory endpointInstanceFactory;
        private readonly IScheduledJobsConfiguration config;
        private readonly IPaymentLogger paymentLogger;

        public AuditDataCleanUpService(IScheduledJobsConfiguration config, IPaymentsDataContext dataContext, IEndpointInstanceFactory endpointInstanceFactory, IPaymentLogger paymentLogger)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.endpointInstanceFactory = endpointInstanceFactory ?? throw new ArgumentNullException(nameof(endpointInstanceFactory));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
        }

        public async Task TriggerAuditDataCleanup()
        {
            var submissionJobsToBeDeletedBatches = await GetSubmissionJobsToBeDeletedBatches(config.CollectionPeriod, config.AcademicYear);

            var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);

            foreach (var batch in submissionJobsToBeDeletedBatches)
            {
                await endpointInstance.Send(config.EarningAuditDataCleanUpQueue, batch).ConfigureAwait(false);
                await endpointInstance.Send(config.DataLockAuditDataCleanUpQueue, batch).ConfigureAwait(false);
                await endpointInstance.Send(config.FundingSourceAuditDataCleanUpQueue, batch).ConfigureAwait(false);
                await endpointInstance.Send(config.RequiredPaymentAuditDataCleanUpQueue, batch).ConfigureAwait(false);
            }
        }

        private async Task SplitBatchAndEnqueueMessages(SubmissionJobsToBeDeletedBatch batch, string queueName)
        {
            var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);

            foreach (var jobsToBeDeletedModel in batch.JobsToBeDeleted)
            {
                await endpointInstance.Send(queueName, new SubmissionJobsToBeDeletedBatch { JobsToBeDeleted = new[] { jobsToBeDeletedModel } }).ConfigureAwait(false);
            }
        }

        private async Task AuditDataCleanUp(Func<IList<SqlParameter>, string, string, Task> deleteAuditData, SubmissionJobsToBeDeletedBatch batch, string queueName)
        {
            try
            {
                var sqlParameters = batch.JobsToBeDeleted.ToSqlParameters();

                var deleteMethodName = deleteAuditData.Method.Name;

                paymentLogger.LogInfo($"Started {deleteMethodName}");

                var sqlParamName = string.Join(", ", sqlParameters.Select(pn => pn.ParameterName));
                var paramValues = string.Join(", ", sqlParameters.Select(pn => pn.Value));

                await deleteAuditData(sqlParameters, sqlParamName, paramValues);

                paymentLogger.LogInfo($"Finished {deleteMethodName}");
            }
            catch (Exception e)
            {
                //we have already tried in single batch mode nothing more can be done here
                if (batch.JobsToBeDeleted.Length == 1)
                {
                    paymentLogger.LogWarning($"Error Deleting Audit Data, internal Exception {e}");
                    throw;
                }

                //if SQL TimeOut or Dead-lock and we haven't already tried with single item Mode then try again with Batch Split into single items
                if (e.IsTimeOutException() || e.IsDeadLockException())
                {
                    paymentLogger.LogWarning($"Starting Audit Data Deletion in Single Item mode");
                    
                    await SplitBatchAndEnqueueMessages(batch, queueName);
                }
            }
        }

        public async Task EarningEventAuditDataCleanUp(SubmissionJobsToBeDeletedBatch batch)
        {
            await AuditDataCleanUp(DeleteEarningEventData, batch, config.EarningAuditDataCleanUpQueue);
        }

        public async Task FundingSourceEventAuditDataCleanUp(SubmissionJobsToBeDeletedBatch batch)
        {
            await AuditDataCleanUp(DeleteFundingSourceEvent, batch, config.FundingSourceAuditDataCleanUpQueue);
        }

        public async Task RequiredPaymentEventAuditDataCleanUp(SubmissionJobsToBeDeletedBatch batch)
        {
            await AuditDataCleanUp(DeleteRequiredPaymentEvent, batch, config.RequiredPaymentAuditDataCleanUpQueue);
        }

        public async Task DataLockEventAuditDataCleanUp(SubmissionJobsToBeDeletedBatch batch)
        {
            await AuditDataCleanUp(DeleteDataLockEvent, batch, config.DataLockAuditDataCleanUpQueue);
        }

        private async Task DeleteEarningEventData(IList<SqlParameter> sqlParameters, string sqlParamName, string paramValues)
        {
            var earningEventPeriodCount = await dataContext.Database.ExecuteSqlCommandAsync(
                    $@"DELETE Payments2.EarningEventPeriod 
                       FROM Payments2.EarningEventPeriod AS EEP 
                           INNER JOIN Payments2.EarningEvent AS EE ON EEP.EarningEventId = EE.EventId 
                       WHERE EE.JobId IN ({sqlParamName})",
                       sqlParameters);

            paymentLogger.LogInfo($"DELETED {earningEventPeriodCount} earningEventPeriods for JobIds {paramValues}");

            var earningEventPriceEpisodeCount = await dataContext.Database.ExecuteSqlCommandAsync(
                    $@"DELETE Payments2.EarningEventPriceEpisode 
                       FROM Payments2.EarningEventPriceEpisode AS EEPE 
                          INNER JOIN Payments2.EarningEvent AS EE ON EEPE.EarningEventId = EE.EventId 
                       WHERE EE.JobId IN ({sqlParamName})",
                       sqlParameters);

            paymentLogger.LogInfo($"DELETED {earningEventPriceEpisodeCount} earningEventPriceEpisodes for JobIds {paramValues}");

            var earningEventCount = await dataContext.Database.ExecuteSqlCommandAsync(
                    $"DELETE Payments2.EarningEvent WHERE JobId IN ({sqlParamName})",
                    sqlParameters);

            paymentLogger.LogInfo($"DELETED {earningEventCount} EarningEvents for JobIds {paramValues}");
        }

        private async Task DeleteFundingSourceEvent(IList<SqlParameter> sqlParameters, string sqlParamName, string paramValues)
        {
            var fundingSourceEventCount = await dataContext.Database.ExecuteSqlCommandAsync(
                    $"DELETE Payments2.FundingSourceEvent WHERE JobId IN ({sqlParamName})",
                    sqlParameters);

            paymentLogger.LogInfo($"DELETED {fundingSourceEventCount} FundingSourceEvents for JobIds {paramValues}");
        }

        private async Task DeleteRequiredPaymentEvent(IList<SqlParameter> sqlParameters, string sqlParamName, string paramValues)
        {
            var requiredPaymentEventCount = await dataContext.Database.ExecuteSqlCommandAsync(
                    $"DELETE Payments2.RequiredPaymentEvent WHERE JobId IN ({sqlParamName})",
                    sqlParameters);

            paymentLogger.LogInfo($"DELETED {requiredPaymentEventCount} RequiredPaymentEvents for JobIds {paramValues}");
        }

        private async Task DeleteDataLockEvent(IList<SqlParameter> sqlParameters, string sqlParamName, string paramValues)
        {
            var dataLockEventNonPayablePeriodFailuresCount = await dataContext.Database.ExecuteSqlCommandAsync(
                    $@"DELETE Payments2.DataLockEventNonPayablePeriodFailures 
                       FROM Payments2.DataLockEventNonPayablePeriodFailures AS DLENPPF 
                           INNER JOIN Payments2.DataLockEventNonPayablePeriod AS DLENPP ON DLENPPF.DataLockEventNonPayablePeriodId = DLENPP.DataLockEventNonPayablePeriodId 
                           INNER JOIN Payments2.DataLockEvent AS DL ON DLENPP.DataLockEventId = DL.EventId
                       WHERE DL.JobId IN ({sqlParamName})",
                       sqlParameters);

            paymentLogger.LogInfo($"DELETED {dataLockEventNonPayablePeriodFailuresCount} DataLockEventNonPayablePeriodFailures for JobIds {paramValues}");

            var dataLockEventNonPayablePeriodCount = await dataContext.Database.ExecuteSqlCommandAsync(
                    $@"DELETE Payments2.DataLockEventNonPayablePeriod 
                       FROM Payments2.DataLockEventNonPayablePeriod AS DLENPP
                           INNER JOIN Payments2.DataLockEvent AS DL ON DLENPP.DataLockEventId = DL.EventId
                       WHERE DL.JobId IN ({sqlParamName})",
                       sqlParameters);

            paymentLogger.LogInfo($"DELETED {dataLockEventNonPayablePeriodCount} DataLockEventNonPayablePeriods for JobIds {paramValues}");

            var dataLockEventPayablePeriodCount = await dataContext.Database.ExecuteSqlCommandAsync(
                    $@"DELETE Payments2.DataLockEventPayablePeriod 
                       FROM Payments2.DataLockEventPayablePeriod AS DLEPP
                           INNER JOIN Payments2.DataLockEvent AS DL ON DLEPP.DataLockEventId = DL.EventId
                       WHERE DL.JobId IN ({sqlParamName})",
                       sqlParameters);

            paymentLogger.LogInfo($"DELETED {dataLockEventPayablePeriodCount} DataLockEventPayablePeriods for JobIds {paramValues}");

            var dataLockEventPriceEpisodeCount = await dataContext.Database.ExecuteSqlCommandAsync(
                    $@"DELETE Payments2.DataLockEventPriceEpisode 
                       FROM Payments2.DataLockEventPriceEpisode AS DLEPP
                          INNER JOIN Payments2.DataLockEvent AS DL ON DLEPP.DataLockEventId = DL.EventId
                       WHERE DL.JobId IN ({sqlParamName})",
                       sqlParameters);

            paymentLogger.LogInfo($"DELETED {dataLockEventPriceEpisodeCount} DataLockEventPriceEpisodes for JobIds {paramValues}");

            var dataLockEventCount = await dataContext.Database.ExecuteSqlCommandAsync(
                    $"DELETE Payments2.DataLockEvent WHERE JobId IN ({sqlParamName})",
                    sqlParameters);

            paymentLogger.LogInfo($"DELETED {dataLockEventCount} DataLockEvents for JobIds {paramValues}");
        }

        private async Task<IEnumerable<SubmissionJobsToBeDeletedBatch>> GetSubmissionJobsToBeDeletedBatches(string collectionPeriod, string academicYear)
        {
            // ReSharper disable once ConvertToConstant.Local
            var selectJobsToBeDeleted = @"
                IF OBJECT_ID('tempdb..#JobDataToBeDeleted') IS NOT NULL DROP TABLE #JobDataToBeDeleted;

                SELECT JobId INTO #JobDataToBeDeleted FROM Payments2.EarningEvent
                WHERE CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear
                UNION
                SELECT JobId FROM Payments2.RequiredPaymentEvent
                WHERE CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear
                UNION
                SELECT JobId FROM Payments2.FundingSourceEvent
                WHERE CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear
                UNION
                SELECT JobId FROM Payments2.DataLockEvent
                WHERE CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear;

                -- keep all successful Jobs, 
                DELETE FROM #JobDataToBeDeleted WHERE JobId IN ( SELECT DcJobId FROM Payments2.LatestSuccessfulJobs );

                -- and Keep all in progress and Timed-out and Failed on DC Jobs
                DELETE FROM #JobDataToBeDeleted WHERE JobId IN ( SELECT DcJobId FROM Payments2.Job WHERE [Status] in (1, 4, 5) );

                -- and any jobs completed on our side but DC status is unknown
                DELETE FROM #JobDataToBeDeleted WHERE JobId IN ( SELECT DcJobId FROM Payments2.Job Where [Status] in (2, 3) AND Dcjobsucceeded IS NULL);

                SELECT JobId AS DcJobId FROM #JobDataToBeDeleted";

            return (await dataContext.SubmissionJobsToBeDeleted
                .FromSql(selectJobsToBeDeleted,
                    new SqlParameter("collectionPeriod", collectionPeriod),
                    new SqlParameter("academicYear", academicYear))
                .ToListAsync())
                .ToBatch(100);
        }
    }
}