using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{
    public interface ISubmissionJobsDataContext
    {
        DbSet<LatestSuccessfulJobModel> LatestSuccessfulJobs { get; }
    }

    public class SubmissionJobsDataContext : DbContext, ISubmissionJobsDataContext
    {
        public DbSet<LatestSuccessfulJobModel> LatestSuccessfulJobs { get; protected set; }

        public SubmissionJobsDataContext(DbContextOptions contextOptions) : base(contextOptions)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfiguration(new LatestSuccessfulJobModelConfiguration());
        }
    }
}
