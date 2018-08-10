using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Autofac;
using Autofac.Integration.ServiceFabric;
using AutoMapper;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.PaymentsDue.Application.Data;
using SFA.DAS.Payments.PaymentsDue.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.PaymentsDue.Application.Repositories;
using SFA.DAS.Payments.PaymentsDue.Model.Entities;

[assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)]

namespace SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueService
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
                builder.RegisterActor<ApprenticeshipPaymentsDueService>();

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
            //builder.RegisterType<ReliableCollectionCache<IEnumerable<PaymentEntity>>>().AsImplementedInterfaces();
            builder.RegisterType<PaymentHistoryRepository>().AsImplementedInterfaces();
            //builder.RegisterType<ActorProxyFactory>().AsImplementedInterfaces();
            //builder.RegisterType<ActorService>().AsSelf();
        }

        private static void RegisterMap(ContainerBuilder builder)
        {
            builder.Register(context =>
                {
                    //var profiles = context.Resolve<IEnumerable<Profile>>();
                    var config = AutoMapperConfigurationFactory.CreateMappingConfig();
                    //var config = new MapperConfiguration(x =>
                    //{
                    //    // Load in all our AutoMapper profiles that have been registered
                    //    foreach (var profile in profiles)
                    //    {
                    //        x.AddProfile(profile);
                    //    }
                    //});

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
