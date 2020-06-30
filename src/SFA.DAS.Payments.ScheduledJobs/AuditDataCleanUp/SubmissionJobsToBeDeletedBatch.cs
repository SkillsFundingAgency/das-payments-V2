using NServiceBus;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp
{
    public class SubmissionJobsToBeDeletedBatch : IMessage
    {
        public SubmissionJobsToBeDeletedModel[] JobsToBeDeleted { get; set; }
    }
}
