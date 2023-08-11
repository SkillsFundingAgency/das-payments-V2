using System;
using System.Threading;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

namespace SFA.DAS.Payments.EarningEvents.EarningEventsService
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                using (ServiceFabricContainerFactory.CreateContainerForStatelessService<EarningEvents.EarningEventsService.EarningEventsService>())
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