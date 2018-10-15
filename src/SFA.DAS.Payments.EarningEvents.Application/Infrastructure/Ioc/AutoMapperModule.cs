using System;
using System.Collections.Generic;
using Autofac;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Application.Infrastructure.Configuration;

namespace SFA.DAS.Payments.EarningEvents.Application.Infrastructure.Ioc
{
    public class AutoMapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx =>
                {
                    var config = AutoMapperConfigurationFactory.CreateMappingConfig();
                    return config;
                })
                .SingleInstance() // We only need one instance
                .AutoActivate() // Create it on ContainerBuilder.Build()
                .AsSelf(); // Bind it to its own type

            builder.Register(tempContext =>
                {
                    var ctx = tempContext.Resolve<IComponentContext>();
                    var config = ctx.Resolve<MapperConfiguration>();

                    // Create our mapper using our configuration above
                    return config.CreateMapper();
                })
                .As<IMapper>(); // Bind it to the IMapper interface

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(type => type.IsClass && type.IsPublic && !type.IsAbstract && type.IsAssignableTo<Profile>())
                .As<Profile>();

            builder.Register(c => new MapperConfiguration(cfg => {
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
