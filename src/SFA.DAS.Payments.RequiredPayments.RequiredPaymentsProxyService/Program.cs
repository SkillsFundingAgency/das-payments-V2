using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.ServiceFabric;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payment.ServiceFabric.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService
{
    internal static class Program
    {
       
        private static void Main()
        {
            try
            {
                var builder = new ContainerBuilder();

                RegisterServices(builder);

                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<RequiredPaymentsProxyService>("SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyServiceType");

                using (builder.Build())
                {
                    Thread.Sleep(Timeout.Infinite);
                }

            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<ActorProxyFactory>().As<IActorProxyFactory>();
            builder.RegisterType<RequiredPaymentsProxyService>().AsImplementedInterfaces();

            // TODO: use configuration 
            builder.Register((c, p) => new ServiceConfig
            {
                IncomingEndpointName = "sfa-das-payments-paymentsdue-proxyservice",
                OutgoingEndpointName = "sfa-das-payments-paymentsdue-proxyservice-out",
                DestinationEndpointName = "sfa-das-payments-requiredpayments-proxyservice",
                StorageConnectionString = "UseDevelopmentStorage=true"
            }
            ).AsImplementedInterfaces();

            builder.Register((c, p) =>
            {
                var config = c.Resolve<IServiceConfig>();
                return new EndpointCommunicationSender<IPaymentsDueEvent>(
                    config.OutgoingEndpointName,
                    config.StorageConnectionString,
                    config.DestinationEndpointName,
                    c.Resolve<ILifetimeScope>()
                );
            }).As<IEndpointCommunicationSender<IPaymentsDueEvent>>();

            builder.Register((c, p) =>
            {
                var config = c.Resolve<IServiceConfig>();
                return new EndpointCommunicationListener<IPayableEarningEvent>(
                    config.IncomingEndpointName,
                    config.StorageConnectionString,
                    c.Resolve<ILifetimeScope>()
                );
            }).As<IEndpointCommunicationListener<IPayableEarningEvent>>();

        }
    }
}
