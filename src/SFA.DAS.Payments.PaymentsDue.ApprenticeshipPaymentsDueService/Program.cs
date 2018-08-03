using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using AutoMapper;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.PaymentsDue.Application.Repositories;
using SFA.DAS.Payments.PaymentsDue.Model.Entities;

namespace SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueService
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
                // This line registers an Actor Service to host your actor class with the Service Fabric runtime.
                // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                // are automatically populated when you build this project.
                // For more information, see https://aka.ms/servicefabricactorsplatform

                var container = BuildContainer();

                ActorRuntime.RegisterActorAsync<ApprenticeshipPaymentsDueService>
                    (
                        (context, actorType) => new ActorService
                        (
                            context,
                            actorType,
                            (service, id) =>
                            {
                                using (var scope = container.BeginLifetimeScope())
                                {
                                    return new ApprenticeshipPaymentsDueService(service, id, scope.Resolve<IPaymentHistoryRepository>());
                                }
                            })
                    )
                    .GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            RegisterServices(builder);
            RegisterMap(builder);
            var c = builder.Build();
            return c;
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<ReliableCollectionCache<IEnumerable<PaymentEntity>>>().AsImplementedInterfaces();
            builder.RegisterType<PaymentHistoryRepository>().AsImplementedInterfaces();
        }

        private static void RegisterMap(ContainerBuilder builder)
        {
            builder.Register(c => new MapperConfiguration(cfg =>
            {
                foreach (var profile in c.Resolve<IEnumerable<Profile>>())
                {
                    cfg.AddProfile(profile);
                }
            })).AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<MapperConfiguration>()
                    .CreateMapper(c.Resolve))
                .As<IMapper>()
                .InstancePerLifetimeScope();
        }
    }
}
