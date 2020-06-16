using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp
{
    public static class Extensions
    {
        public static IEnumerable<SubmissionJobsToBeDeletedBatch> ToBatch(this IEnumerable<SubmissionJobsToBeDeletedModel> items, int maxItems)
        {
            return items.Select((submissionJobs, index) => new { submissionJobs, index })
                .GroupBy(x => x.index / maxItems)
                .Select(g => new SubmissionJobsToBeDeletedBatch
                {
                    JobsToBeDeleted = g.Select(batch => batch.submissionJobs).ToArray()
                });
        }

        public static IList<SqlParameter> ToSqlParameters(this IEnumerable<SubmissionJobsToBeDeletedModel> items)
        {
            return items.Select((item, index) => new SqlParameter($"@DcJobId{index}", item.DcJobId)).ToList();
        }
    }
}