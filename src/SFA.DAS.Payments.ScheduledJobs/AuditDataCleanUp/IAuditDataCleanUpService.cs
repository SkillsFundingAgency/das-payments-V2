using System.Threading.Tasks;

namespace SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp
{
    public interface IAuditDataCleanUpService
    {
        Task EarningEventAuditDataCleanUp(SubmissionJobsToBeDeletedBatch jobsToBeDeleted);
        Task FundingSourceEventAuditDataCleanUp(SubmissionJobsToBeDeletedBatch batch);
        Task RequiredPaymentEventAuditDataCleanUp(SubmissionJobsToBeDeletedBatch batch);
        Task DataLockEventAuditDataCleanUp(SubmissionJobsToBeDeletedBatch batch);
        Task TriggerAuditDataCleanup();
    }
}