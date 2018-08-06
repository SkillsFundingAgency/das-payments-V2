using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using Autofac.Integration.ServiceFabric;
using AutoMapper;
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
                var builder = new ContainerBuilder();

                RegisterServices(builder);
                RegisterMap(builder);

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
