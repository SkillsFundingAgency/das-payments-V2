using Autofac;
using AutoMapper;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;

namespace SFA.DAS.Payments.FundingSource.Application.Infrastructure.Ioc
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
        }
    }
}