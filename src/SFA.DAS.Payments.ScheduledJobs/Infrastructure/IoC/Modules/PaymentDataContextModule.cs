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
                            optionsBuilder => optionsBuilder.CommandTimeout(590)).Options;  //Max Timeout for Function execution is 10 min so setting SQL timeOut to 9.50 == 590 sec
                    return new PaymentsDataContext(dbContextOptions);
                })
                .As<IPaymentsDataContext>()
                .InstancePerLifetimeScope();
        }
    }
}