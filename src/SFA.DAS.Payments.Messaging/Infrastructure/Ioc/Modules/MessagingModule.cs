using Autofac;
using SFA.DAS.Payments.Messaging.Serialization;
using SFA.DAS.Payments.Messaging.Serialization.NServiceBus;

namespace SFA.DAS.Payments.Messaging.Infrastructure.Ioc.Modules
{
    public class MessagingModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageDeserializer>()
                .As<IMessageDeserializer>();

            builder.RegisterType<DefaultApplicationMessageModifier>()
                .As<IApplicationMessageModifier>()
                .IfNotRegistered(typeof(IApplicationMessageModifier));
        }
    }
}