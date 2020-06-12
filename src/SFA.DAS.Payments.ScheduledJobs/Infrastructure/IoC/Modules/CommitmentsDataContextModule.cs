using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules
{
    public class CommitmentsDataContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    var dbContextOptions = new DbContextOptionsBuilder<CommitmentsDataContext>()
                        .UseSqlServer(configHelper.GetConnectionString("CommitmentsConnectionString"),
                            optionsBuilder => optionsBuilder.CommandTimeout(270)).Options;
                    return new CommitmentsDataContext(dbContextOptions);
                })
                .As<ICommitmentsDataContext>()
                .InstancePerLifetimeScope();
        }
    }
}