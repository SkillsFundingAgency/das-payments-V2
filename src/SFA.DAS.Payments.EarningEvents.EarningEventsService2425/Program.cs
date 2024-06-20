using System;
using System.Threading;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

namespace SFA.DAS.Payments.EarningEvents.EarningEventsService2425
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                using (ServiceFabricContainerFactory.CreateContainerForStatelessService<EarningEvents.EarningEventsService2425.EarningEventsService2425>())
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
