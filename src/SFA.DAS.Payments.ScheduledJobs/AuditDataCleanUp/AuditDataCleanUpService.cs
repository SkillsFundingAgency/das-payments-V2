using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;

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
        private readonly IPaymentLogger paymentLogger;

        public AuditDataCleanUpService([Inject] IPaymentsDataContext dataContext, [Inject] IPaymentLogger paymentLogger)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
        }

        public async Task EarningEventAuditDataCleanUp()
        {
            var jobsToBeDeleted = await dataContext.SubmissionJobsToBeDeleted.ToListAsync();
            foreach (var job in jobsToBeDeleted)
            {
                try
                {
                    await dataContext.Database.ExecuteSqlCommandAsync(@"
                    DELETE Payments2.EarningEventPeriod 
                    FROM Payments2.EarningEventPeriod AS EEP 
                        INNER JOIN Payments2.EarningEvent AS EE ON EEP.EarningEventId = EE.EventId 
                    WHERE EE.JobId = @DcJobId", 
                        new SqlParameter("DcJobId", job.DcJobId));

                    await dataContext.Database.ExecuteSqlCommandAsync(@"
                    DELETE Payments2.EarningEventPriceEpisode 
                    FROM Payments2.EarningEventPriceEpisode AS EEPE 
                        INNER JOIN Payments2.EarningEvent AS EE ON EEPE.EarningEventId = EE.EventId 
                    WHERE EE.JobId = @DcJobId",
                        new SqlParameter("DcJobId", job.DcJobId));

                    await dataContext.Database.ExecuteSqlCommandAsync("DELETE Payments2.EarningEvent WHERE JobId = @DcJobId", new SqlParameter("DcJobId", job.DcJobId));
                }
                catch (Exception e)
                {
                    paymentLogger.LogWarning($"Error Deleting EarningEvent Audit Data, internal Exception {e}");
                }
            }
        }
        
        public async Task FundingSourceEventAuditDataCleanUp()
        {
            var jobsToBeDeleted = await dataContext.SubmissionJobsToBeDeleted.ToListAsync();
            foreach (var job in jobsToBeDeleted)
            {
                try
                {
                    await dataContext.Database.ExecuteSqlCommandAsync("DELETE Payments2.FundingSourceEvent WHERE JobId = @DcJobId", new SqlParameter("DcJobId", job.DcJobId));
                }
                catch (Exception e)
                {
                    paymentLogger.LogWarning($"Error Deleting FundingSourceEvent Audit Data, internal Exception {e}");
                }
            }
        }
        
        public async Task RequiredPaymentEventAuditDataCleanUp()
        {
            var jobsToBeDeleted = await dataContext.SubmissionJobsToBeDeleted.ToListAsync();
            foreach (var job in jobsToBeDeleted)
            {
                try
                {
                    await dataContext.Database.ExecuteSqlCommandAsync("DELETE Payments2.RequiredPaymentEvent WHERE JobId = @DcJobId", new SqlParameter("DcJobId", job.DcJobId));
                }
                catch (Exception e)
                {
                    paymentLogger.LogWarning($"Error Deleting RequiredPaymentEvent Audit Data, internal Exception {e}");
                }
            }
        }
        
        public async Task DataLockEventAuditDataCleanUp()
        {
            var jobsToBeDeleted = await dataContext.SubmissionJobsToBeDeleted.ToListAsync();
            foreach (var job in jobsToBeDeleted)
            {
                try
                {
                    await dataContext.Database.ExecuteSqlCommandAsync(@"DELETE Payments2.DataLockEventNonPayablePeriodFailures 
                        FROM Payments2.DataLockEventNonPayablePeriodFailures AS DLENPPF 
                            INNER JOIN Payments2.DataLockEventNonPayablePeriod AS DLENPP ON DLENPPF.DataLockEventNonPayablePeriodId = DLENPP.DataLockEventNonPayablePeriodId 
                            INNER JOIN Payments2.DataLockEvent AS DL ON DLENPP.DataLockEventId = DL.EventId
                        WHERE DL.JobId = @DcJobId",
                        new SqlParameter("DcJobId", job.DcJobId));

                    await dataContext.Database.ExecuteSqlCommandAsync(@"DELETE Payments2.DataLockEventNonPayablePeriod 
                        FROM Payments2.DataLockEventNonPayablePeriod AS DLENPP
                            INNER JOIN Payments2.DataLockEvent AS DL ON DLENPP.DataLockEventId = DL.EventId
                        WHERE DL.JobId = @DcJobId",
                        new SqlParameter("DcJobId", job.DcJobId));

                    await dataContext.Database.ExecuteSqlCommandAsync(@"DELETE Payments2.DataLockEventPayablePeriod 
                        FROM Payments2.DataLockEventPayablePeriod AS DLEPP
                            INNER JOIN Payments2.DataLockEvent AS DL ON DLEPP.DataLockEventId = DL.EventId
                        WHERE DL.JobId = @DcJobId",
                        new SqlParameter("DcJobId", job.DcJobId));

                    await dataContext.Database.ExecuteSqlCommandAsync(@"DELETE Payments2.DataLockEventPriceEpisode 
                        FROM Payments2.DataLockEventPriceEpisode AS DLEPP
                            INNER JOIN Payments2.DataLockEvent AS DL ON DLEPP.DataLockEventId = DL.EventId
                        WHERE DL.JobId = @DcJobId",
                        new SqlParameter("DcJobId", job.DcJobId));

                    await dataContext.Database.ExecuteSqlCommandAsync("DELETE Payments2.DataLockEvent WHERE JobId = @DcJobId", new SqlParameter("DcJobId", job.DcJobId));
                }
                catch (Exception e)
                {
                    paymentLogger.LogWarning($"Error Deleting DataLockEvent Audit Data, internal Exception {e}");
                }
            }
        }
    }
}