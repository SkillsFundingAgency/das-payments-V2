using Autofac;
using Autofac.Integration.ServiceFabric;
using AutoMapper;
using SFA.DAS.Payments.Core.LoggingHelper;
using SFA.DAS.Payments.RequiredPayments.Application.Data;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using System;
using System.Threading;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                // https://alexmg.com/posts/introducing-the-autofac-integration-for-service-fabric

                var builder = new ContainerBuilder();

                RegisterServices(builder);
                RegisterMap(builder);

                builder.RegisterServiceFabricSupport();
                builder.RegisterActor<RequiredPaymentsService>();

                using (builder.Build())
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            // TODO: use configuration
            builder.Register((c, p) => new ServiceConfig
            {
                IncomingEndpointName = "sfa-das-payments-paymentsdue-proxyservice",
                OutgoingEndpointName = "sfa-das-payments-paymentsdue-proxyservice-out",
                DestinationEndpointName = "sfa-das-payments-requiredpayments-proxyservice",
                StorageConnectionString = "UseDevelopmentStorage=true",
                LoggerConnectionstring = "Server=.;Database=AppLog;User Id=SFActor; Password=SFActor;"
            }).AsImplementedInterfaces();

            //Register Logger
            builder.Register((c, p) =>
            {
                var config = c.Resolve<IServiceConfig>();
                return new LoggerOptions
                {
                    LoggerConnectionstring = config.LoggerConnectionstring
                };
            }).As<LoggerOptions>().SingleInstance();
            builder.RegisterType<VersionInfo>().As<IVersionInfo>().SingleInstance();
            builder.RegisterModule<LoggerModule>();

            // Register services
            builder.RegisterType<PaymentHistoryRepository>().AsImplementedInterfaces();
        }

        private static void RegisterMap(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                //var profiles = context.Resolve<IEnumerable<Profile>>();
                var config = AutoMapperConfigurationFactory.CreateMappingConfig();

                return config;
            }).SingleInstance() // We only need one instance
                .AutoActivate() // Create it on ContainerBuilder.Build()
                .AsSelf(); // Bind it to its own type

            // HACK: IComponentContext needs to be resolved again as 'tempContext' is only temporary. See http://stackoverflow.com/a/5386634/718053
            builder.Register(tempContext =>
            {
                var ctx = tempContext.Resolve<IComponentContext>();
                var config = ctx.Resolve<MapperConfiguration>();

                // Create our mapper using our configuration above
                return config.CreateMapper();
            }).As<IMapper>(); // Bind it to the IMapper interface

            builder.Register((c, p) => new DedsContext());
        }
    }
}