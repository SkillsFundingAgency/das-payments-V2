using Autofac;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.RequiredPayments.Application.Data;

namespace SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Ioc
{
    public class DataContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var configHelper = c.Resolve<IConfigurationHelper>();
                return new RequiredPaymentsDataContext(configHelper.GetConnectionString("PaymentsConnectionString"));
            }).As<IRequiredPaymentsDataContext>();
        }
    }
}