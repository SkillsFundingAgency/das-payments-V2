using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.PeriodEnd.Application.Repositories;
using SFA.DAS.Payments.PeriodEnd.Application.Services;
using SFA.DAS.Payments.PeriodEnd.Data;
using SFA.DAS.Payments.PeriodEnd.Function.Infrastructure.Configuration;

namespace SFA.DAS.Payments.PeriodEnd.Function.Infrastructure.IoC.Modules
{
    public class FunctionsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PeriodEndRepository>().As<IPeriodEndRepository>().InstancePerLifetimeScope();

            builder.Register((c, p) =>
                {
                    var config = c.Resolve<ISubmissionMetricsConfiguration>();
                    var dbContextOptions = new DbContextOptionsBuilder<PeriodEndDataContext>()
                        .UseSqlServer(config.PaymentsConnectionString, 
                  optionsBuilder => optionsBuilder.CommandTimeout(270)).Options;
                    return new PeriodEndDataContext(dbContextOptions);
                })
                .As<IPeriodEndDataContext>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SubmissionJobsService>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
