using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using Module = Autofac.Module;

namespace SFA.DAS.Payments.EarningEvents.Application.Infrastructure.Ioc
{
    public class AutoMapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.Register(ctx =>
            //    {
            //        var config = AutoMapperConfigurationFactory.CreateMappingConfig();
            //        return config;
            //    })
            //    .SingleInstance() // We only need one instance
            //    .AutoActivate() // Create it on ContainerBuilder.Build()
            //    .AsSelf(); // Bind it to its own type

            //builder.Register(tempContext =>
            //    {
            //        var ctx = tempContext.Resolve<IComponentContext>();
            //        var config = ctx.Resolve<MapperConfiguration>();

            //        // Create our mapper using our configuration above
            //        return config.CreateMapper();
            //    })
            //    .As<IMapper>(); // Bind it to the IMapper interface

            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //builder.RegisterAssemblyTypes(assemblies)
            //    .Where(type => type.IsClass && type.IsPublic && !type.IsAbstract && type.IsAssignableTo<Profile>())
            //    .As<Profile>();

            //builder.Register(c => new MapperConfiguration(cfg => {
            //    foreach (var profile in c.Resolve<IEnumerable<Profile>>())
            //    {
            //        cfg.AddProfile(profile);
            //    }
            //})).AsSelf().SingleInstance();

            //builder.Register(c => c.Resolve<MapperConfiguration>()
            //        .CreateMapper(c.Resolve))
            //    .As<IMapper>()
            //    .InstancePerLifetimeScope();

            var assembly = GetType().Assembly;
            builder.RegisterAssemblyTypes(assembly)
                .Where(type => type.IsClass && type.IsPublic && !type.IsAbstract && type.IsAssignableTo<Profile>())
                .As<Profile>()
                .SingleInstance();

            //builder.RegisterType<OnProgrammeEarningValueResolver>()
            //    .AsSelf()
            //    .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(IValueResolver<,,>))
                .AsSelf()
                .SingleInstance();


            builder.Register(c => new MapperConfiguration(
                    cfg =>
                {
                    cfg.AddProfiles(GetType().Assembly);
                    //var profiles = c.Resolve<IEnumerable<Profile>>().ToList();
                    //profiles.ForEach(cfg.AddProfile());
                }))
                .AsSelf()
                .SingleInstance();

            builder.Register(c => new Mapper(c.Resolve<MapperConfiguration>(), ContainerFactory.Container.Resolve))
                .As<IMapper>();
            //.InstancePerLifetimeScope();
        }
    }
}
