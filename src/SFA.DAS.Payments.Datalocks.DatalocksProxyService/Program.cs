using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Castle.Core.Internal;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

[assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)]

namespace SFA.DAS.Payments.Datalocks.DatalocksProxyService
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
                using (ServiceFabricContainerFactory.CreateContainerForStatelessService<DatalocksProxyService>())
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
