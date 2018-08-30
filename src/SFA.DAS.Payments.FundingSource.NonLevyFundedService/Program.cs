using Castle.Core.Internal;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

[assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)]

namespace SFA.DAS.Payments.FundingSource.NonLevyFundedService
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                var builder = ServiceFabricContainerFactory.CreateBuilderForStatelessService<NonLevyFundedService>();

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