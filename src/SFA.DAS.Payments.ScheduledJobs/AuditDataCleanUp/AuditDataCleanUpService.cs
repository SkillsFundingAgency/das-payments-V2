using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration;

namespace SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp
{
    public interface IAuditDataCleanUpService
    {
        Task EarningEventAuditDataCleanUp();
        Task FundingSourceEventAuditDataCleanUp();
        Task RequiredPaymentEventAuditDataCleanUp();
        Task DataLockEventAuditDataCleanUp();
    }

    public class AuditDataCleanUpService : IAuditDataCleanUpService
    {
        private readonly IPaymentsDataContext dataContext;
        private readonly IScheduledJobsConfiguration configuration;
        private readonly IPaymentLogger paymentLogger;

        public AuditDataCleanUpService([Inject] IPaymentsDataContext dataContext, [Inject] IScheduledJobsConfiguration configuration, [Inject] IPaymentLogger paymentLogger)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
        }

        public async Task EarningEventAuditDataCleanUp()
        {
            await AuditDataCleanUp(DeleteEarningEventData);
        }

        public async Task FundingSourceEventAuditDataCleanUp()
        {
            await AuditDataCleanUp(DeleteFundingSourceEvent);
        }

        public async Task RequiredPaymentEventAuditDataCleanUp()
        {
            await AuditDataCleanUp(DeleteRequiredPaymentEvent);
        }

        public async Task DataLockEventAuditDataCleanUp()
        {
            await  AuditDataCleanUp(DeleteDataLockEvent);
        }

        private async Task AuditDataCleanUp(Func<IList<SqlParameter>, string, string, Task> deleteAuditData)
        {
            try
            {
                var jobsToBeDeleted = await GetSubmissionJobsToBeDeleted();

                var methodName = deleteAuditData.Method.Name;

                paymentLogger.LogInfo($"Started {methodName}");

                foreach (var sqlParameters in jobsToBeDeleted)
                {
                    var sqlParamName = string.Join(", ", sqlParameters.Select(pn => pn.ParameterName));
                    var paramValues = string.Join(", ", sqlParameters.Select(pn => pn.Value));
                    
                    await deleteAuditData(sqlParameters, sqlParamName, paramValues);
                }

                paymentLogger.LogInfo($"Finished {methodName}");
            }
            catch (Exception e)
            {
                paymentLogger.LogWarning($"Error Deleting Audit Data, internal Exception {e}");
            }
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

        private async Task<IEnumerable<IList<SqlParameter>>> GetSubmissionJobsToBeDeleted()
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
                DELETE FROM #JobDataToBeDeleted WHERE JobId IN ( SELECT DcJobId FROM Payments2.Job Where [Status] in (2, 3) AND (Dcjobsucceeded IS NULL OR Dcjobsucceeded = 0));

                SELECT JobId AS DcJobId FROM #JobDataToBeDeleted";

            return ( await dataContext.SubmissionJobsToBeDeleted
                .FromSql(selectJobsToBeDeleted, 
                    new SqlParameter("collectionPeriod", configuration.CollectionPeriod), 
                    new SqlParameter("academicYear", configuration.AcademicYear)).ToListAsync() )
                .ToSqlParameterBatch(100);
        }
    }

    public static class BatchExtensions
    {
        public static IEnumerable<IList<SqlParameter>> ToSqlParameterBatch(this IEnumerable<SubmissionJobsToBeDeletedModel> items, int maxItems)
        {
            return items.Select((item, index) => new { item, index })
                        .GroupBy(x => x.index / maxItems)
                        .Select(g => g.Select((job, index) => new SqlParameter($"@DcJobId{index}", job.item.DcJobId)).ToList());
        }
    }
}