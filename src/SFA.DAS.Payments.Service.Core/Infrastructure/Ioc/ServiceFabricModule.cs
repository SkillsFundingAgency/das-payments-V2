using Autofac;
using Autofac.Integration.ServiceFabric;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc
{
    public class ServiceFabricModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterServiceFabricSupport();
        }
    }
}