using Autofac;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;

namespace SFA.DAS.Payments.EarningEvents.Application.Infrastructure.Ioc
{
    public class SerialisationModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService, ISerializationService>();
            builder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>();
        }
    }
}
