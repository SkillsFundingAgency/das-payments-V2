using System;
using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules
{
    public class PaymentsDataContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    var context = new PaymentsDataContext(configHelper.GetConnectionString("PaymentsConnectionString"));
                    context.Database.SetCommandTimeout(TimeSpan.FromSeconds(270));
                    return context;
                })
                .As<IPaymentsDataContext>()
                .InstancePerLifetimeScope();
        }
    }
}