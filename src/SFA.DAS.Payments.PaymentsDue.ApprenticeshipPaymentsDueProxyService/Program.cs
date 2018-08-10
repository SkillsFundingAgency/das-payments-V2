using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Autofac;
using Autofac.Core;
using Autofac.Integration.ServiceFabric;
using Castle.Core.Internal;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using SFA.DAS.Payment.ServiceFabric.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.PaymentsDue.Domain.Services;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

[assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)]

namespace SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueProxyService
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                var builder = new ContainerBuilder();

                RegisterServices(builder);

                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<ApprenticeshipPaymentsDueProxyService>("SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueProxyServiceType");

                using (builder.Build())
                {
                    Thread.Sleep(Timeout.Infinite);
                }

                //ServiceRuntime.RegisterServiceAsync("SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueProxyServiceType",
                //    context => new ApprenticeshipPaymentsDueProxyService(context)).GetAwaiter().GetResult();

                //ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(ApprenticeshipPaymentsDueProxyService).Name);

                //// Prevents this host process from terminating so services keep running.
                //Thread.Sleep(Timeout.Infinite);
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
            builder.RegisterType<ApprenticeshipKeyService>().AsImplementedInterfaces();

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
