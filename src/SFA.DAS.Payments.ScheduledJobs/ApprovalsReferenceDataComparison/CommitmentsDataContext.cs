using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.ScheduledJobs.ApprovalsReferenceDataComparison
{
    public class CommitmentsDataContext : DbContext, ICommitmentsDataContext
    {
        public CommitmentsDataContext(DbContextOptions<CommitmentsDataContext> options) : base(options)
        {
        }

        public DbSet<ApprenticeshipModel> Apprenticeships { get; }
    }
}