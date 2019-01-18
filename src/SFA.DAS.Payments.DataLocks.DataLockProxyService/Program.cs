using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Castle.Core.Internal;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

[assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)]
namespace SFA.DAS.Payments.DataLocks.DataLockProxyService
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                using (ServiceFabricContainerFactory.CreateContainerForStatelessService<DataLockProxyService>())
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
