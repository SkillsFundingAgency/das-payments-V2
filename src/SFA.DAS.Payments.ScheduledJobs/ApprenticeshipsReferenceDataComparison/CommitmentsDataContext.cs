using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.ScheduledJobs.ApprenticeshipsReferenceDataComparison
{
    public class CommitmentsDataContext : DbContext, ICommitmentsDataContext
    {
        public CommitmentsDataContext(DbContextOptions<CommitmentsDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.ApplyConfiguration(new ApprenticeshipConfiguration());
        }

        public DbSet<ApprenticeshipModel> Apprenticeship { get; set; }
    }
}