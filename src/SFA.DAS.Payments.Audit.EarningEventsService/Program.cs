using System;
using System.Diagnostics;
using System.Threading;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

namespace SFA.DAS.Payments.Audit.EarningEventsService
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
                using (ServiceFabricContainerFactory.CreateContainerForStatefulService<EarningEventsService>())
                {
                    //ServiceEventSource.Current.ServiceTypeRegistered(
                    //    Process.GetCurrentProcess().Id,
                    //    typeof(EarningEventsService).Name);
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
