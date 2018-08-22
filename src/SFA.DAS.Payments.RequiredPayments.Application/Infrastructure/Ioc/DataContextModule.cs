using Autofac;
using SFA.DAS.Payments.RequiredPayments.Application.Data;

namespace SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Ioc
{
    public class DataContextModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) => new RequiredPaymentsDataContext()).As<IRequiredPaymentsDataContext>();
        }
    }
}