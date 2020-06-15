using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules
{
    public class PaymentDataContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    var dbContextOptions = new DbContextOptionsBuilder<PaymentsDataContext>()
                        .UseSqlServer(configHelper.GetConnectionString("PaymentsConnectionString"), 
                            optionsBuilder => optionsBuilder.CommandTimeout(540)).Options; //540 = 9 min
                    return new PaymentsDataContext(dbContextOptions);
                })
                .As<IPaymentsDataContext>()
                .InstancePerLifetimeScope();
        }
    }
}