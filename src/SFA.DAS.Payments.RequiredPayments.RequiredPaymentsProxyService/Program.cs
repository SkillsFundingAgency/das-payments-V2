using Autofac;
using Castle.Core.Internal;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

[assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)]

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                var builder = ServiceFabricContainerFactory.CreateBuilderForStatelessService<RequiredPaymentsProxyService>();

                using (builder.Build())
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

    }
}