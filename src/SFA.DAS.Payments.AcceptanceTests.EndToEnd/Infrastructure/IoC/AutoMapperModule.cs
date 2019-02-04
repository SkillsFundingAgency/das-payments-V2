using Autofac;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using Module = Autofac.Module;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Infrastructure.IoC
{
    public class AutoMapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            var assembly = GetType().Assembly;
            builder.RegisterAssemblyTypes(assembly)
                .Where(type => type.IsClass && type.IsPublic && !type.IsAbstract && type.IsAssignableTo<Profile>())
                .As<Profile>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(IValueResolver<,,>))
                .AsSelf()
                .SingleInstance();

            builder.Register(c => new MapperConfiguration(
                    cfg =>
                {
                    cfg.AddProfiles(GetType().Assembly);
                }))
                .AsSelf()
                .SingleInstance();

            builder.Register(c => new Mapper(c.Resolve<MapperConfiguration>(), ContainerFactory.Container.Resolve))
                .As<IMapper>();
        }
    }
}
