using Autofac;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc.Modules
{
    public class EndpointListenerModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.Register((c, p) =>
            //{
            //    var config = c.Resolve<IApplicationConfiguration>();
            //    return new EndpointCommunicationSender(
            //        config.OutgoingEndpointName,
            //        config.StorageConnectionString,
            //        config.DestinationEndpointName,
            //        c.Resolve<ILifetimeScope>()
            //    );
            //}).As<IEndpointCommunicationSender<IPaymentsDueEvent>>();

            builder.Register((c, p) =>
            {
                var config = c.Resolve<IApplicationConfiguration>();
                return new EndpointCommunicationListener(
                    config.EndpointName,
                    config.StorageConnectionString,
                    c.Resolve<ILifetimeScope>()
                );
            }).As<IEndpointCommunicationListener>();
        }
    }
}