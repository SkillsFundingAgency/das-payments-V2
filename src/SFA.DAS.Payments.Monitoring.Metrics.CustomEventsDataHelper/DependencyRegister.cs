using Autofac;
using AzureFunctions.Autofac.Configuration;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules;
using System.Reflection;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper
{
    public class DependencyRegister
    {
        public DependencyRegister(string functionName)
        {
            DependencyInjection.Initialize(RegisterModules, functionName);
        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<TelemetryModule>();
            builder.RegisterModule<SFA.DAS.Payments.Monitoring.Metrics.Function.Infrastructure.IoC.Modules.ConfigurationModule>();
        }
    }
}
