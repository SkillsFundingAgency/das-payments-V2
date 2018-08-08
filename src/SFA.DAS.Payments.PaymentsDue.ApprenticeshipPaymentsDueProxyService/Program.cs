using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Autofac;
using Autofac.Integration.ServiceFabric;
using Castle.Core.Internal;

[assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)]

namespace SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueProxyService
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

                builder.RegisterServiceFabricSupport();
                builder.RegisterStatelessService<ApprenticeshipPaymentsDueProxyService>("SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueProxyServiceType");

                using (builder.Build())
                {
                    Thread.Sleep(Timeout.Infinite);
                }

                //ServiceRuntime.RegisterServiceAsync("SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueProxyServiceType",
                //    context => new ApprenticeshipPaymentsDueProxyService(context)).GetAwaiter().GetResult();

                //ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(ApprenticeshipPaymentsDueProxyService).Name);

                //// Prevents this host process from terminating so services keep running.
                //Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
